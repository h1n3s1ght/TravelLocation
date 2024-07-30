using System;
using System.Collections.Generic;

namespace TravelLocationManagement.Models
{
    public class ListItem
    {
        public Guid ListItemId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Nickname { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ListId { get; set; } // Foreign key for List
        public List List { get; set; } = new List(); // Navigation property
        public Location Location { get; set; } = new Location(); // Navigation property
        public Guid LocationId { get; set; } // Foreign key for Location
        public ICollection<string> Photos { get; set; } = new List<string>(); // List of photo URLs
        public float Rating { get; set; }
        public string Summary { get; set; } = string.Empty;
    }
}
