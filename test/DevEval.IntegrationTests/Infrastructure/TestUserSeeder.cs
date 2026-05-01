using DevEval.Common.Services;
using DevEval.Domain.Entities.User;
using DevEval.Domain.Enums;
using DevEval.Domain.ValueObjects;
using DevEval.ORM.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevEval.IntegrationTests.Infrastructure;

internal sealed class TestUserSeeder
{
    private readonly IServiceProvider _serviceProvider;

    public TestUserSeeder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SeedAuthenticationUserAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

        var existingUser = await dbContext.Users.SingleOrDefaultAsync(user => user.Username == TestAuthentication.Username);
        if (existingUser is not null)
        {
            return;
        }

        var user = new User(
            TestAuthentication.Email,
            TestAuthentication.Username,
            passwordService.HashPassword(TestAuthentication.Password),
            UserRole.Admin)
        {
            Name = new Name("Integration", "Admin")
        };

        user.UpdatePhone("+5511999999999");
        user.UpdateStatus(UserStatus.Active);

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }
}
