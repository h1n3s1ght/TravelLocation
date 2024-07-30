using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace TravelLocationManagement.Models
{
    public class Role : IdentityRole<Guid>
    {
        public Role() : base() { }

        public Role(string roleName) : base(roleName) { }

        public int RoleEntityId { get; set; }
        public string RoleType { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string BasicUser = "BasicUser";
        public const string ProUser = "ProUser";

        public static Role GetRoleByName(string roleName)
        {
            return roleName switch
            {
                Admin => new Role { RoleType = Admin },
                BasicUser => new Role { RoleType = BasicUser },
                ProUser => new Role { RoleType = ProUser },
                _ => throw new KeyNotFoundException($"Role '{roleName}' not found.")
            };
        }
    }
}

