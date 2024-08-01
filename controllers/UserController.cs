using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TravelLocationManagement.Models;
using TravelLocationManagement.DTOs;
using TravelLocationManagement.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Swashbuckle.AspNetCore.Annotations;


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
        [SwaggerOperation(Summary = "Create a new user", Description = "Creates a new user with the specified role.")]
        [SwaggerResponse(200, "User created successfully", typeof(User))]
        [SwaggerResponse(400, "Bad request")]
        public async Task<IActionResult> CreateUser([FromBody, SwaggerRequestBody("User creation payload", Required = true)] UserCreationDTO userData, string? role = "BasicUser")
        {
            if (role != "Admin" && role != "ProUser")
            {
                role = "BasicUser";
            }

            var userRole = await _roleManager.FindByNameAsync(userData.Role.Name);
            if (userRole == null)
            {
                return BadRequest("Role not found.");
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = userData.UserName,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                Email = userData.Email,
                PhoneNumber = userData.PhoneNumber,
                EmailConfirmed = userData.EmailConfirmed, 
                PasswordHash = _passwordHasher.HashPassword(new User(), userData.Password),
                CreatedAt = userData.CreatedAt,
                RoleId = userRole.Id
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, userData.Role.Name);
                return Ok(user);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("retrieve")]
        [SwaggerOperation(Summary = "Retrieve user data", Description = "Retrieves user data based on identifier and password.")]
        [SwaggerResponse(200, "User data retrieved successfully", typeof(UserRetrievalDto))]
        [SwaggerResponse(400, "Bad request")]
        [SwaggerResponse(404, "User not found")]
        public async Task<IActionResult> RetrieveUser([FromBody] UserRetrievalDto retrievalDto)
        {
            User user = null;
            var identifier = retrievalDto.Identifier;

            // First, attempt to find by UserName
            user = await _userManager.FindByNameAsync(identifier);

            // If user is not found by UserName, check other identifiers
            if (user == null)
            {
                switch (identifier)
                {
                    case string email when identifier.Contains("@"):
                        user = await _userManager.FindByEmailAsync(email);
                        break;

                    case string id when Guid.TryParse(identifier, out var userId):
                        user = await _userManager.FindByIdAsync(userId.ToString());
                        break;
                }
            }

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Verify password
            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? string.Empty, retrievalDto.Password);
            if (passwordVerificationResult != PasswordVerificationResult.Success)
            {
                return BadRequest("Invalid password.");
            }

            var userInfo = new
            {
                user.Id,
                user.UserName,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber,
                user.EmailConfirmed,
                user.CreatedAt,
                user.RoleId,
                Role = user.Role?.Name,
                Lists = user.Lists?.Select(l => new
                {
                    l.ListName,
                    l.CreatedAt,
                    l.ListItems
                }),
                SharedLists = user.SharedLists?.Select(sl => new
                {
                    sl.List.ListName,
                    sl.CreatedAt,
                    sl.SharedWithUserId
                })
            };

            return Ok(userInfo);
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
