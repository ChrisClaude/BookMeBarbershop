using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookMe.Application.Entities;

namespace BookMe.Infrastructure.Data.Configurations;

public class BookingEntityTypeConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.TimeSlotId).IsRequired();
        builder.Property(x => x.Status).IsRequired();

        builder.HasOne(x => x.User).WithMany(x => x.Bookings).HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.TimeSlot).WithOne(x => x.Booking).HasForeignKey<Booking>(x => x.TimeSlotId);
    }
}
