using DevEval.Domain.Enums;
using DevEval.Domain.ValueObjects;

namespace DevEval.Domain.Entities.User
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Gets the email of the user.
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the hashed password of the user.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Gets the name details of the user.
        /// </summary>
        public Name? Name { get; set; }

        /// <summary>
        /// Gets the address details of the user.
        /// </summary>
        public Address? Address { get; set; }

        /// <summary>
        /// Gets the phone number of the user.
        /// </summary>
        public string? Phone { get; private set; }

        /// <summary>
        /// Gets the current status of the user.
        /// </summary>
        public UserStatus Status { get; private set; }

        /// <summary>
        /// Gets the role assigned to the user.
        /// </summary>
        public UserRole Role { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="email">The email of the user.</param>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <param name="role">The role of the user.</param>
        public User(string email, string username, string password, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            Email = email;
            Username = username;
            Password = password; // Replace with a hashing function in production
            Role = role;
            Status = UserStatus.Active; // Default to Active
        }

        /// <summary>
        /// Updates the name of the user.
        /// </summary>
        /// <param name="name">The new name.</param>
        public void UpdateName(Name name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void UpdateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            Email = email;
        }

        public void UpdateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));

            Username = username;
        }

        /// <summary>
        /// Updates the address of the user.
        /// </summary>
        /// <param name="address">The new address.</param>
        public void UpdateAddress(Address? address)
        {
            Address = address;
        }

        /// <summary>
        /// Updates the phone number of the user.
        /// </summary>
        /// <param name="phone">The new phone number.</param>
        public void UpdatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be null or empty.", nameof(phone));

            Phone = phone;
        }

        /// <summary>
        /// Updates the password of the user.
        /// </summary>
        /// <param name="password">The new password.</param>
        public void UpdatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            Password = password; // Replace with a hashing function in production
        }

        /// <summary>
        /// Updates the status of the user.
        /// </summary>
        /// <param name="status">The new status.</param>
        public void UpdateStatus(UserStatus status)
        {
            Status = status;
        }

        /// <summary>
        /// Updates the role of the user.
        /// </summary>
        /// <param name="role">The new role.</param>
        public void UpdateRole(UserRole role)
        {
            Role = role;
        }
    }
}
