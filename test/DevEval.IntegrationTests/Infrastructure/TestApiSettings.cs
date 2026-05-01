namespace DevEval.IntegrationTests.Infrastructure;

public sealed record TestApiSettings(
    string ConnectionString,
    string JwtKey,
    string JwtIssuer,
    string JwtAudience);
