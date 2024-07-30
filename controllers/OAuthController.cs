using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TravelLocationManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> OAuthLogin([FromBody] OAuthLoginModel model)
        {
            // Implement OAuth login logic here
            await Task.CompletedTask; // Ensure the method contains an await operator
            return Ok(new { Result = "OAuth login successful" });
        }
    }

    public class OAuthLoginModel
    {
        public string Provider { get; set; } = string.Empty;
        public string IdToken { get; set; } = string.Empty;
    }
}