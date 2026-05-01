using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DevEval.IntegrationTests.Infrastructure;

internal sealed class TestAuthenticationClient
{
    private readonly HttpClient _httpClient;

    public TestAuthenticationClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> LoginAndGetTokenAsync()
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new
        {
            username = TestAuthentication.Username,
            password = TestAuthentication.Password
        });

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (payload is null || string.IsNullOrWhiteSpace(payload.Token))
        {
            throw new InvalidOperationException("Authentication endpoint did not return a valid JWT token.");
        }

        return payload.Token;
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var token = await LoginAndGetTokenAsync();
        var client = _httpClient;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private sealed record LoginResponse(string Token);
}
