namespace DevEval.IntegrationTests.Infrastructure;

public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected IntegrationTestBase(IntegrationTestFixture fixture)
    {
        Fixture = fixture;
    }

    protected IntegrationTestFixture Fixture { get; }

    protected HttpClient CreateApiClient()
    {
        return Fixture.CreateApiClient();
    }

    protected async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = Fixture.CreateApiClient();
        var authClient = new TestAuthenticationClient(client);
        return await authClient.CreateAuthenticatedClientAsync();
    }

    public virtual Task InitializeAsync()
    {
        return Fixture.ResetDatabaseAsync();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
