using Microsoft.AspNetCore.Identity;
using dotenv.net;
using TravelLocationManagement.Models; // Ensure you have the correct namespace for your User model

namespace TravelLocationManagement.Utilities // Correct namespace
{
    public class CustomPasswordHasher : IPasswordHasher<User>
    {
        private readonly string _defaultPasswordHash;

        public CustomPasswordHasher()
        {
            DotEnv.Load();

            _defaultPasswordHash = Environment.GetEnvironmentVariable("DEFAULT_PASSWORD_HASH")
                                  ?? throw new InvalidOperationException("DEFAULT_PASSWORD_HASH environment variable is not set.");
        }

        public string HashPassword(User user, string password)
        {
            if (user == null || string.IsNullOrEmpty(password))
            {
                return _defaultPasswordHash;
            }

            var passwordHasher = new PasswordHasher<User>();
            return passwordHasher.HashPassword(user, password);
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            var passwordHasher = new PasswordHasher<User>();
            return passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }
    }
}