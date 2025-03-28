using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookMe.Application.Entities;

namespace BookMe.Infrastructure.Data.Configurations;

public class TimeSlotEntityTypeConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("TimeSlots");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Start).IsRequired();
        builder.Property(x => x.End).IsRequired();
    }
}
