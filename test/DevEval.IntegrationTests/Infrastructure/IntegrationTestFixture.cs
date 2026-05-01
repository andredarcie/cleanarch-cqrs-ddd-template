namespace DevEval.IntegrationTests.Infrastructure;

public sealed class IntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgresTestDatabase _database = new();
    private DatabaseResetter? _databaseResetter;

    public CustomWebApplicationFactory Factory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _database.StartAsync();
        Factory = new CustomWebApplicationFactory(new TestApiSettings(
            _database.ConnectionString,
            "IntegrationTestsSecretKey1234567890!",
            "DevEval.IntegrationTests",
            "DevEval.IntegrationTests"));

        using var client = Factory.CreateApiClient();

        _databaseResetter = await DatabaseResetter.CreateAsync(_database.ConnectionString);
        await ResetDatabaseAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        ArgumentNullException.ThrowIfNull(_databaseResetter);

        await _databaseResetter.ResetAsync();
        var seeder = new TestUserSeeder(Factory.Services);
        await seeder.SeedAuthenticationUserAsync();
    }

    public HttpClient CreateApiClient()
    {
        return Factory.CreateApiClient();
    }

    public async Task DisposeAsync()
    {
        if (_databaseResetter is not null)
        {
            await _databaseResetter.DisposeAsync();
        }

        Factory?.Dispose();
        await _database.StopAsync();
    }
}
