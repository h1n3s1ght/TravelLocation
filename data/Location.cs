using System;

namespace TravelLocationManagement.Models
{
    public class Location
    {
        public Guid LocationId { get; set; }
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool Visited { get; set; }
        public string? Address { get; set; }
        public string? Coordinates { get; set; }
        public string[] Tags { get; set; } = Array.Empty<string>();
        public float UserRating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User User { get; set; } = default!;
    }
}