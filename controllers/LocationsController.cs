using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TravelLocationManagement.Data;
using TravelLocationManagement.Models;

namespace TravelLocationManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly TravelLocationContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public LocationsController(TravelLocationContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            var locations = await _context.Locations.ToListAsync();
            if (locations == null || !locations.Any())
            {
                return NotFound();
            }
            return Ok(locations);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<object>> GetLocation(Guid id)
        {
            var location = await _context.Locations
                .Include(l => l.LocationAddress)
                .FirstOrDefaultAsync(l => l.LocationId == id);

            if (location == null)
            {
                return NotFound();
            }

            // Fetch additional details
            var publicRatingsAndReviews = await GetPublicRatingsAndReviews(location);
            var weatherForecast = await GetWeatherForecast(location);

            // Add the details to the response
            var locationDetails = new
            {
                Location = location,
                PublicRatingsAndReviews = publicRatingsAndReviews,
                WeatherForecast = weatherForecast
            };

            return Ok(locationDetails);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<Location>> PostLocation(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLocation", new { id = location.LocationId }, location);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> PutLocation(Guid id, Location location)
        {
            if (id != location.LocationId)
            {
                return BadRequest();
            }

            _context.Entry(location).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteLocation(Guid id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LocationExists(Guid id)
        {
            return _context.Locations.Any(e => e.LocationId == id);
        }

        private async Task<object?> GetPublicRatingsAndReviews(Location location)
        {
            if (location == null || string.IsNullOrEmpty(location.PlaceId))
            {
                return null;
            }

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync($"https://maps.googleapis.com/maps/api/place/details/json?place_id={location.PlaceId}&key=YOUR_GOOGLE_API_KEY");
            var json = JObject.Parse(response);
            return json["result"]?["reviews"];
        }

        private async Task<object?> GetWeatherForecast(Location location)
        {
            if (location == null || location.Latitude == null || location.Longitude == null)
            {
                return null;
            }

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.GetStringAsync($"https://api.openweathermap.org/data/2.5/forecast?lat={location.Latitude}&lon={location.Longitude}&appid=YOUR_OPENWEATHERMAP_API_KEY");
            var json = JObject.Parse(response);
            return json["list"];
        }
    }
}