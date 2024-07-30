using TravelLocationManagement.Data;
using TravelLocationManagement.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace TravelLocationManagement.Controllers
{
    public class MainController
    {
        private readonly UserController _userController;

        public MainController(TravelLocationContext context, UserManager<User> userManager, RoleManager<Role> roleManager, IPasswordHasher<User> passwordHasher)
        {
            _userController = new UserController(context, userManager, roleManager, passwordHasher);
        }

        public async Task CreateUser(User userData)
        {
            await _userController.CreateUser(userData);
        }

        public async Task DeleteUser(Guid userId)
        {
            await _userController.DeleteUser(userId);
        }
    }
}
