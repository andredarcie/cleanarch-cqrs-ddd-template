using System.ComponentModel;
using FluentResults;
using MediatR;

namespace DevEval.Application.Auth.Commands
{
    public class LoginUserCommand : IRequest<Result<string>>
    {
        [DefaultValue("admin")]
        public string Username { get; set; }

        [DefaultValue("Admin@123")]
        public string Password { get; set; }

        public LoginUserCommand(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}