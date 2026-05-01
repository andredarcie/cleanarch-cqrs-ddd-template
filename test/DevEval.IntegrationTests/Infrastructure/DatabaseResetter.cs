using Npgsql;
using Respawn;
using Respawn.Graph;

namespace DevEval.IntegrationTests.Infrastructure;

internal sealed class DatabaseResetter : IAsyncDisposable
{
    private readonly NpgsqlConnection _connection;
    private readonly Respawner _respawner;

    private DatabaseResetter(NpgsqlConnection connection, Respawner respawner)
    {
        _connection = connection;
        _respawner = respawner;
    }

    public static async Task<DatabaseResetter> CreateAsync(string connectionString)
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = new[] { "public" },
            TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
        });

        return new DatabaseResetter(connection, respawner);
    }

    public Task ResetAsync()
    {
        return _respawner.ResetAsync(_connection);
    }

    public ValueTask DisposeAsync()
    {
        return _connection.DisposeAsync();
    }
}
