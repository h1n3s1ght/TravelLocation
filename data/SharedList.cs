using System;

namespace TravelLocationManagement.Models
{
    public class SharedList
    {
        public Guid ListId { get; set; }
        public Guid OwnerId { get; set; }
        public Guid SharedWithUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Owner { get; set; } = default!;
        public User SharedWithUser { get; set; } = default!;
    }
}