using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TravelLocationManagement.Models;
using TravelLocationManagement.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TravelLocationManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly TravelLocationContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserController(
            TravelLocationContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("{role?}")]
        public async Task<IActionResult> CreateUser([FromBody] User userData, string? role = "BasicUser")
        {
            if (role != "Admin" && role != "ProUser")
            {
                role = "BasicUser";
            }

            var userRole = await _roleManager.FindByNameAsync(role);
            if (userRole == null)
            {
                return BadRequest("Role not found.");
            }

            var user = new User
            {
                UserName = userData.UserName,
                Email = userData.Email,
                EmailConfirmed = userData.EmailConfirmed,
                PasswordHash = _passwordHasher.HashPassword(userData, userData.PasswordHash ?? ""),
                OAuthProvider = userData.OAuthProvider,
                OAuthId = userData.OAuthId,
                CreatedAt = DateTime.UtcNow,
                RoleId = userRole.Id
            };

            var result = await _userManager.CreateAsync(user, userData.PasswordHash ?? "");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return Ok(user);
            }

            return BadRequest(result.Errors);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok();
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("{identifier}")]
        public async Task<IActionResult> GetUser(string identifier)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Lists)
                .ThenInclude(l => l.ListItems)
                .Include(u => u.SharedLists)
                .ThenInclude(sl => sl.List)
                .FirstOrDefaultAsync(u => u.UserName == identifier || u.Email == identifier);

            if (user == null)
            {
                return NotFound();
            }

            var userInfo = new
            {
                user.UserName,
                user.Email,
                user.EmailConfirmed,
                user.OAuthProvider,
                user.OAuthId,
                user.CreatedAt,
                Role = user.Role?.Name,
                Lists = user.Lists.Select(l => new
                {
                    l.ListName,
                    l.CreatedAt,
                    l.ListItems,
                    l.SharedWith
                }),
                SharedLists = user.SharedLists.Select(sl => new
                {
                    sl.List.ListName,
                    sl.CreatedAt,
                    sl.SharedWithUserId
                })
            };

            return Ok(userInfo);
        }

        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            return Ok(users);
        }
    }
}
