using System;
using Swashbuckle.AspNetCore.Annotations;


namespace TravelLocationManagement.DTOs
{
     public class UserCreationDTO
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public int PhoneNumber { get; set; }
        public string? Password { get; set; }
        public RoleDTO? Role { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }

        public UserCreationDTO()
        {
            EmailConfirmed = false;
            CreatedAt = DateTime.UtcNow;
        }


        // Or initialize in a constructor
        public UserCreationDTO(string userName, string firstName, string lastName, string email, string password)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }
    }

    public class RoleDTO
    {
        public string? Name { get; set; }
    }
}
