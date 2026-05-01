using System.Net;
using System.Net.Http.Json;
using DevEval.IntegrationTests.Fixtures;
using DevEval.IntegrationTests.Infrastructure;

namespace DevEval.IntegrationTests.Scenarios.Auth;

[Collection(IntegrationTestCollection.Name)]
public sealed class LoginTests : IntegrationTestBase
{
    public LoginTests(IntegrationTestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Login_WhenCredentialsAreValid_ShouldReturnJwtToken()
    {
        using var client = CreateApiClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = TestAuthentication.Username,
            password = TestAuthentication.Password
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();

        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload.Token));
    }

    private sealed record LoginResponse(string Token);
}
