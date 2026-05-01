using DevEval.WebApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace DevEval.IntegrationTests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly TestApiSettings _settings;

    public CustomWebApplicationFactory(TestApiSettings settings)
    {
        _settings = settings;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("IntegrationTests");
        builder.UseSetting("ConnectionStrings:PostgreSqlConnection", _settings.ConnectionString);
        builder.UseSetting("Jwt:Key", _settings.JwtKey);
        builder.UseSetting("Jwt:Issuer", _settings.JwtIssuer);
        builder.UseSetting("Jwt:Audience", _settings.JwtAudience);
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSqlConnection"] = _settings.ConnectionString,
                ["Jwt:Key"] = _settings.JwtKey,
                ["Jwt:Issuer"] = _settings.JwtIssuer,
                ["Jwt:Audience"] = _settings.JwtAudience
            });
        });
    }

    public HttpClient CreateApiClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
    }
}
