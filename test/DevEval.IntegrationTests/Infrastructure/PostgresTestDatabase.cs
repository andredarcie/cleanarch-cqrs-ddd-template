using System.ComponentModel;
using System.Diagnostics;
using Npgsql;

namespace DevEval.IntegrationTests.Infrastructure;

internal sealed class PostgresTestDatabase
{
    private const string ExternalConnectionStringEnvironmentVariable = "DEVEVAL_TEST_POSTGRES_CONNECTION";
    private const int PostgreSqlContainerPort = 5432;
    private readonly string _containerName = $"deveval-integration-tests-{Guid.NewGuid():N}";
    private string? _connectionString;
    private int _postgresPort;
    private bool _usesExternalDatabase;

    public string ConnectionString =>
        _connectionString
        ?? throw new InvalidOperationException("Integration test database has not been initialized.");

    public async Task StartAsync()
    {
        var externalConnectionString = Environment.GetEnvironmentVariable(ExternalConnectionStringEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(externalConnectionString))
        {
            _usesExternalDatabase = true;
            _connectionString = externalConnectionString;
            await WaitUntilReadyAsync();
            return;
        }

        await EnsureDockerIsAvailableAsync();

        await RunDockerCommandAsync(
            "run",
            "-d",
            "--rm",
            "--name",
            _containerName,
            "-e",
            "POSTGRES_DB=DevEvalIntegrationTests",
            "-e",
            "POSTGRES_USER=postgres",
            "-e",
            "POSTGRES_PASSWORD=postgres",
            "-p",
            $"127.0.0.1::{PostgreSqlContainerPort}",
            "postgres:15");

        _postgresPort = await ResolveMappedPortAsync();
        _connectionString = $"Host=127.0.0.1;Port={_postgresPort};Database=DevEvalIntegrationTests;Username=postgres;Password=postgres";
        await WaitUntilReadyAsync();
    }

    public async Task StopAsync()
    {
        if (_usesExternalDatabase)
        {
            return;
        }

        try
        {
            await RunDockerCommandAsync("rm", "-f", _containerName);
        }
        catch
        {
            // Best-effort cleanup.
        }
    }

    private static async Task EnsureDockerIsAvailableAsync()
    {
        try
        {
            await RunDockerCommandAsync("version", "--format", "{{.Server.Version}}");
        }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException)
        {
            throw new InvalidOperationException(
                "Integration tests require Docker with the engine running, or the environment variable " +
                $"'{ExternalConnectionStringEnvironmentVariable}' pointing to a PostgreSQL instance. " +
                $"Docker is unavailable in the current environment. Details: {ex.Message}");
        }
    }

    private async Task WaitUntilReadyAsync()
    {
        const int maxAttempts = 30;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await using var connection = new NpgsqlConnection(ConnectionString);
                await connection.OpenAsync();
                return;
            }
            catch when (attempt < maxAttempts)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        throw new InvalidOperationException("PostgreSQL container did not become ready in time.");
    }

    private async Task<int> ResolveMappedPortAsync()
    {
        const int maxAttempts = 10;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var output = await RunDockerCommandAsync("port", _containerName, $"{PostgreSqlContainerPort}/tcp");
            var parts = output.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length > 0 && int.TryParse(parts[^1], out var port))
            {
                return port;
            }

            if (attempt < maxAttempts)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250));
                continue;
            }

            throw new InvalidOperationException($"Could not resolve mapped PostgreSQL port from docker output: {output}");
        }

        throw new InvalidOperationException("Could not resolve mapped PostgreSQL port.");
    }

    private static async Task<string> RunDockerCommandAsync(params string[] args)
    {
        var startInfo = new ProcessStartInfo("docker")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var arg in args)
        {
            startInfo.ArgumentList.Add(arg);
        }

        using var process = new Process { StartInfo = startInfo };
        process.Start();

        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Docker command failed: docker {string.Join(' ', args)}{Environment.NewLine}{stderr}");
        }

        return stdout.Trim();
    }
}
