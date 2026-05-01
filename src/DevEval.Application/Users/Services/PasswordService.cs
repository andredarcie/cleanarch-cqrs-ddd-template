using System.Security.Cryptography;

namespace DevEval.Common.Services
{
    /// <summary>
    /// Service for securely hashing and verifying passwords.
    /// Implements PBKDF2 (Password-Based Key Derivation Function 2) for strong password hashing.
    /// </summary>
    public class PasswordService : IPasswordService
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 10000;

        /// <summary>
        /// Hashes a password using PBKDF2 with a randomly generated salt.
        /// </summary>
        /// <param name="password">The plaintext password to hash.</param>
        /// <returns>
        /// A string in the format "salt:hash", where both salt and hash are Base64-encoded.
        /// </returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            var salt = GenerateSalt();
            var hash = GenerateHash(password, salt);
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifies a password against a stored hash.
        /// </summary>
        /// <param name="hashedPassword">The stored password hash in the format "salt:hash".</param>
        /// <param name="password">The plaintext password to verify.</param>
        /// <returns>True if the password matches the hash; otherwise, false.</returns>
        /// <exception cref="FormatException">Thrown if the hashed password format is invalid.</exception>
        public bool VerifyPassword(string hashedPassword, string password)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Inputs cannot be null or empty.");

            var parts = hashedPassword.Split(':');
            if (parts.Length != 2)
                throw new FormatException("Hashed password format is invalid.");

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);
            var newHash = GenerateHash(password, salt);

            return CryptographicOperations.FixedTimeEquals(hash, newHash);
        }

        /// <summary>
        /// Generates a random salt for password hashing.
        /// </summary>
        /// <returns>A randomly generated salt as a byte array.</returns>
        private byte[] GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            var salt = new byte[SaltSize];
            rng.GetBytes(salt);
            return salt;
        }

        /// <summary>
        /// Generates a hashed value for a password using the given salt.
        /// </summary>
        /// <param name="password">The plaintext password.</param>
        /// <param name="salt">The salt to use for hashing.</param>
        /// <returns>The derived key as a byte array.</returns>
        private byte[] GenerateHash(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);
        }
    }
}
