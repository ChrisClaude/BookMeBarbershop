using System;
using Microsoft.EntityFrameworkCore;
using BookMe.Application.Entities;
using BookMe.Infrastructure.Data.Configurations;

namespace BookMe.Infrastructure.Data;

public class BookMeContext : DbContext
{
    public BookMeContext(DbContextOptions<BookMeContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RoleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new BookingEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TimeSlotEntityTypeConfiguration());
    }
}
