using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelLocationManagement.Models;
using System;

public class TravelLocationContext : IdentityDbContext<User, Role, Guid>
{
    public TravelLocationContext(DbContextOptions<TravelLocationContext> options)
        : base(options)
    {
    }

    public new DbSet<User> Users { get; set; }
    public DbSet<Location> Locations { get; set; }
    public new DbSet<Role> Roles { get; set; }
    public DbSet<List> Lists { get; set; }
    public DbSet<ListItem> ListItems { get; set; }
    public DbSet<SharedList> SharedLists { get; set; }
    public DbSet<Address> Addresses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<List>()
            .HasMany(l => l.ListItems)
            .WithOne(li => li.List)
            .HasForeignKey(li => li.ListId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SharedList>()
            .HasKey(sl => new { sl.ListId, sl.SharedWithUserId });

        modelBuilder.Entity<SharedList>()
            .HasOne(sl => sl.SharedWithUser)
            .WithMany(u => u.SharedLists)
            .HasForeignKey(sl => sl.SharedWithUserId);

        modelBuilder.Entity<SharedList>()
            .HasOne(sl => sl.List)
            .WithMany()
            .HasForeignKey(sl => sl.ListId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}