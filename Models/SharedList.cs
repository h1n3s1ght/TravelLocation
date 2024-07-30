using System;

namespace TravelLocationManagement.Models
{
    public class SharedList
    {
        public Guid ListId { get; set; }
        public List List { get; set; } = null!;
        public Guid UserId { get; set; }
        public User Owner { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public User SharedWithUser { get; set; } = default!;
        public Guid SharedWithUserId { get; set; }
    }
}
