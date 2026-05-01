using DevEval.Application.Users.Commands;
using DevEval.Application.Users.Dtos;
using DevEval.Domain.Enums;
using DevEval.Domain.Repositories;

namespace DevEval.WebApi.Utils
{
    public static class AdminUserInitializer
    {
        public static async Task EnsureAdminUserExists(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();

            var existingAdmin = await repository.GetByUsernameAsync("admin");
            if (existingAdmin != null)
            {
                Console.WriteLine("Admin user already exists.");
                return;
            }

            var createAdminCommand = new CreateUserCommand
            {
                Email = "admin@deveval.com",
                Username = "admin",
                Password = "Admin@123",
                Name = new NameDto { FirstName = "Admin", LastName = "User" },
                Address = null,
                Phone = "+0000000000",
                Status = UserStatus.Active,
                Role = UserRole.Admin
            };

            var createdAdmin = await mediator.Send(createAdminCommand, CancellationToken.None);

            if (createdAdmin != null)
            {
                Console.WriteLine("Admin user created successfully.");
            }
            else
            {
                Console.WriteLine("Failed to create admin user.");
            }
        }
    }
}
