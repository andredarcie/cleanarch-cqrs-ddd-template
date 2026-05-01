using System.ComponentModel;
using System.Diagnostics;

namespace DevEval.IntegrationTests.Infrastructure;

internal sealed class KafkaTestBroker
{
    private const string ExternalBootstrapServersEnvironmentVariable = "DEVEVAL_TEST_KAFKA_BOOTSTRAP_SERVERS";
    private const int KafkaExternalContainerPort = 9094;
    private readonly string _containerName = $"deveval-kafka-integration-tests-{Guid.NewGuid():N}";
    private int _mappedPort;
    private bool _usesExternalBroker;

    public string BootstrapServers =>
        _usesExternalBroker
            ? Environment.GetEnvironmentVariable(ExternalBootstrapServersEnvironmentVariable)!
            : $"127.0.0.1:{_mappedPort}";

    public async Task StartAsync()
    {
        var externalBootstrapServers = Environment.GetEnvironmentVariable(ExternalBootstrapServersEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(externalBootstrapServers))
        {
            _usesExternalBroker = true;
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
            "KAFKA_NODE_ID=0",
            "-e",
            "KAFKA_PROCESS_ROLES=controller,broker",
            "-e",
            "KAFKA_CONTROLLER_QUORUM_VOTERS=0@127.0.0.1:9093",
            "-e",
            "KAFKA_LISTENERS=PLAINTEXT://:9092,CONTROLLER://:9093,EXTERNAL://:9094",
            "-e",
            "KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://127.0.0.1:9092,EXTERNAL://127.0.0.1:9094",
            "-e",
            "KAFKA_LISTENER_SECURITY_PROTOCOL_MAP=PLAINTEXT:PLAINTEXT,CONTROLLER:PLAINTEXT,EXTERNAL:PLAINTEXT",
            "-e",
            "KAFKA_CONTROLLER_LISTENER_NAMES=CONTROLLER",
            "-e",
            "KAFKA_INTER_BROKER_LISTENER_NAME=PLAINTEXT",
            "-e",
            "KAFKA_AUTO_CREATE_TOPICS_ENABLE=true",
            "-e",
            "KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR=1",
            "-e",
            "KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR=1",
            "-e",
            "KAFKA_TRANSACTION_STATE_LOG_MIN_ISR=1",
            "-e",
            "CLUSTER_ID=MkU3OEVBNTcwNTJENDM2Qk",
            "-p",
            $"127.0.0.1::{KafkaExternalContainerPort}",
            "apache/kafka:3.8.0");

        _mappedPort = await ResolveMappedPortAsync();
        await WaitUntilReadyAsync();
    }

    public async Task StopAsync()
    {
        if (_usesExternalBroker)
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
                $"'{ExternalBootstrapServersEnvironmentVariable}' pointing to a Kafka broker. " +
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
                await RunDockerCommandAsync("exec", _containerName, "/opt/kafka/bin/kafka-topics.sh", "--bootstrap-server", "127.0.0.1:9092", "--list");
                return;
            }
            catch when (attempt < maxAttempts)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        throw new InvalidOperationException("Kafka container did not become ready in time.");
    }

    private async Task<int> ResolveMappedPortAsync()
    {
        const int maxAttempts = 10;

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var output = await RunDockerCommandAsync("port", _containerName, $"{KafkaExternalContainerPort}/tcp");
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

            throw new InvalidOperationException($"Could not resolve mapped Kafka port from docker output: {output}");
        }

        throw new InvalidOperationException("Could not resolve mapped Kafka port.");
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
