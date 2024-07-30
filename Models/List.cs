using System;
using System.Collections.Generic;

namespace TravelLocationManagement.Models
{
    public class List
    {
        public Guid ListId { get; set; }
        public string ListName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public User Owner { get; set; } = default!;
        public Guid OwnerId { get; set; }

        public ICollection<ListItem> ListItems { get; set; } = new List<ListItem>();
        public ICollection<User> SharedWith { get; set; } = new List<User>();
    }
}
