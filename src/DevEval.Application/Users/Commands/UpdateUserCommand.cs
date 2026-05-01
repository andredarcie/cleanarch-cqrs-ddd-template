using DevEval.Application.Users.Dtos;
using DevEval.Domain.Enums;
using FluentResults;
using MediatR;

namespace DevEval.Application.Users.Commands
{
    public class UpdateUserCommand : IRequest<Result<UserDto>>
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public NameDto Name { get; set; } = new();
        public AddressDto Address { get; set; } = new();
        public string Phone { get; set; } = string.Empty;
        public UserStatus Status { get; set; }
        public UserRole Role { get; set; }
    }
}
