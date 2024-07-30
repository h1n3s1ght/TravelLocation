using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace TravelLocationManagement.Models
{
    public class User : IdentityUser<Guid>
    {
        // Custom properties
        public string? OAuthProvider { get; set; } = string.Empty;
        public string? OAuthId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public ICollection<List> Lists { get; set; } = new List<List>();

        // Nullable Role property
        public Guid RoleId { get; set; }
        public Role? Role { get; set; } // Making Role nullable to avoid warning CS8618

        // Ensure non-nullability for overridden properties
        public override string? UserName { get; set; } = string.Empty;
        public override string? Email { get; set; } = string.Empty;
        public override bool EmailConfirmed { get; set; } = false;
        public override string? PasswordHash { get; set; } = string.Empty;

        // Users that this user has shared lists with
        public ICollection<SharedList> SharedLists { get; set; } = new List<SharedList>();
    }
}
