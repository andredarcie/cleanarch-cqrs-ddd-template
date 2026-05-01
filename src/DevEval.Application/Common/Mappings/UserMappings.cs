using DevEval.Application.Users.Commands;
using DevEval.Application.Users.Dtos;
using DevEval.Domain.Entities.User;
using DevEval.Domain.ValueObjects;

namespace DevEval.Application.Common.Mappings;

internal static class UserMappings
{
    public static User ToEntity(this CreateUserCommand command)
    {
        var user = new User(command.Email, command.Username, command.Password, command.Role)
        {
            Name = command.Name.ToValueObject(),
            Address = command.Address.ToValueObject()
        };

        user.UpdatePhone(command.Phone);
        user.UpdateStatus(command.Status);
        user.UpdateRole(command.Role);

        return user;
    }

    public static void ApplyTo(this UpdateUserCommand command, User user)
    {
        user.UpdateEmail(command.Email);
        user.UpdateUsername(command.Username);
        user.UpdatePassword(command.Password);
        user.UpdateName(command.Name.ToValueObject());
        user.UpdateAddress(command.Address.ToValueObject());
        user.UpdatePhone(command.Phone);
        user.UpdateStatus(command.Status);
        user.UpdateRole(command.Role);
    }

    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Name = user.Name.ToDto(),
            Address = user.Address.ToDto(),
            Phone = user.Phone ?? string.Empty,
            Status = user.Status,
            Role = user.Role
        };
    }

    private static Name ToValueObject(this NameDto dto)
    {
        return new Name(dto.FirstName, dto.LastName);
    }

    private static Address? ToValueObject(this AddressDto? dto)
    {
        if (dto is null)
        {
            return null;
        }

        return new Address(
            dto.City,
            dto.Street,
            dto.Number,
            dto.ZipCode,
            dto.Geolocation.ToValueObject());
    }

    private static Geolocation ToValueObject(this GeolocationDto? dto)
    {
        return dto is null ? Geolocation.Empty : new Geolocation(dto.Lat, dto.Long);
    }

    private static NameDto? ToDto(this Name? name)
    {
        return name is null
            ? null
            : new NameDto
            {
                FirstName = name.FirstName,
                LastName = name.LastName
            };
    }

    private static AddressDto? ToDto(this Address? address)
    {
        return address is null
            ? null
            : new AddressDto
            {
                City = address.City,
                Street = address.Street,
                Number = address.Number,
                ZipCode = address.ZipCode,
                Geolocation = address.Geolocation.ToDto()
            };
    }

    private static GeolocationDto? ToDto(this Geolocation? geolocation)
    {
        return geolocation is null
            ? null
            : new GeolocationDto
            {
                Lat = geolocation.Lat,
                Long = geolocation.Long
            };
    }
}
