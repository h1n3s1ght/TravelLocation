using Microsoft.EntityFrameworkCore;
using TravelLocationManagement.Models;

namespace TravelLocationManagement.Data
{
    public class TravelLocationContext : DbContext
    {
        public TravelLocationContext(DbContextOptions<TravelLocationContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SharedList> SharedLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Locations)
                .WithOne(l => l.User)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.SharedLists)
                .WithOne(s => s.Owner)
                .HasForeignKey(s => s.OwnerId);

            modelBuilder.Entity<SharedList>()
                .HasOne(s => s.SharedWithUser)
                .WithMany()
                .HasForeignKey(s => s.SharedWithUserId);
        }
    }
}
