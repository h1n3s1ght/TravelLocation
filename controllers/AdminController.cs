using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelLocationManagement.Data;
using TravelLocationManagement.Models;

namespace TravelLocationManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly TravelLocationContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(TravelLocationContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/admin/users
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/admin/users/{role}
        [HttpGet("users/{role}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            return Ok(users);
        }

        // GET: api/admin/lists
        [HttpGet("lists")]
        public async Task<ActionResult<IEnumerable<SharedList>>> GetLists()
        {
            return await _context.SharedLists.Include(sl => sl.List).Include(sl => sl.SharedWithUser).ToListAsync();
        }
    }
}