using System;
using System.Collections.Generic;

namespace TravelLocationManagement.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string OAuthProvider { get; set; } = string.Empty;
        public string OAuthId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public ICollection<Location> Locations { get; set; } = new List<Location>();
        public ICollection<SharedList> SharedLists { get; set; } = new List<SharedList>();
    }
}

