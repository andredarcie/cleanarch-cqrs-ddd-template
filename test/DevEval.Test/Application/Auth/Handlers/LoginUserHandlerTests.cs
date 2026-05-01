using Application.Users.Handlers;
using DevEval.Application.Auth.Commands;
using DevEval.Application.Common.Errors;
using DevEval.Common.Services;
using DevEval.Domain.Entities.User;
using DevEval.Domain.Enums;
using DevEval.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;

namespace DevEval.Test.Application.Auth.Handlers
{
    public class LoginUserHandlerTests
    {
        private readonly IConfiguration _configurationMock;
        private readonly IUserRepository _userRepositoryMock;
        private readonly IPasswordService _passwordServiceMock;
        private readonly LoginUserHandler _handler;

        public LoginUserHandlerTests()
        {
            _configurationMock = Substitute.For<IConfiguration>();
            _configurationMock["Jwt:Key"].Returns("thisisasupersecureandverylongkey12345678");
            _configurationMock["Jwt:Issuer"].Returns("TestIssuer");
            _configurationMock["Jwt:Audience"].Returns("TestAudience");

            _userRepositoryMock = Substitute.For<IUserRepository>();
            _passwordServiceMock = Substitute.For<IPasswordService>();

            _handler = new LoginUserHandler(_configurationMock, _userRepositoryMock, _passwordServiceMock);
        }

        [Fact]
        public async Task Handle_ValidCredentials_ShouldReturnJwtToken()
        {
            // Arrange
            var command = new LoginUserCommand("admin", "password");
            var user = new User("admin@example.com", "admin", "hashedpassword", UserRole.Admin);

            _userRepositoryMock.GetByUsernameAsync(command.Username).Returns(user);
            _passwordServiceMock.VerifyPassword(user.Password, command.Password).Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            var jwtHandler = new JwtSecurityTokenHandler();
            Assert.True(jwtHandler.CanReadToken(result.Value));

            var jwtToken = jwtHandler.ReadJwtToken(result.Value);
            Assert.Contains(jwtToken.Claims, c => c.Type == "UserId" && c.Value == user.Id.ToString());
            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Username);
        }

        [Fact]
        public async Task Handle_InvalidCredentials_ShouldReturnUnauthorizedError()
        {
            // Arrange
            var command = new LoginUserCommand("wronguser", "wrongpassword");
            _userRepositoryMock.GetByUsernameAsync(command.Username).Returns((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.IsType<UnauthorizedError>(result.Errors.First());
            Assert.Equal("Invalid username or password", result.Errors.First().Message);
        }

        [Fact]
        public async Task Handle_WrongPassword_ShouldReturnUnauthorizedError()
        {
            // Arrange
            var command = new LoginUserCommand("admin", "wrongpassword");
            var user = new User("admin@example.com", "admin", "hashedpassword", UserRole.Admin);

            _userRepositoryMock.GetByUsernameAsync(command.Username).Returns(user);
            _passwordServiceMock.VerifyPassword(user.Password, command.Password).Returns(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsFailed);
            Assert.IsType<UnauthorizedError>(result.Errors.First());
            Assert.Equal("Invalid username or password", result.Errors.First().Message);
        }
    }
}
