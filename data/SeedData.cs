using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using TravelLocationManagement.Models;

namespace TravelLocationManagement.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
            var context = serviceProvider.GetRequiredService<TravelLocationContext>();

            // Check if the roles exist, if not create them
            string[] roleNames = { "Admin", "BasicUser", "ProUser" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }

            // Check if the database has been seeded with any users
            if (await context.Users.AnyAsync())
            {
                return;   // DB has been seeded
            }

            // Create a default Admin user
            var adminUser = new User
            {
                UserName = "admin",
                Email = "admin@example.com",
                EmailConfirmed = true,
            };

            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? throw new ArgumentException("ADMIN_PASSWORD environment variable is not set.");
            var user = await userManager.FindByEmailAsync(adminUser.Email);

            if (user == null)
            {
                var createAdminUser = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdminUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Additional seeding logic for other roles and users if necessary
        }
    }
}
