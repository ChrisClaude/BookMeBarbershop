using BookMe.Application.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMe.Infrastructure.Data.Configurations;

public class TimeSlotEntityTypeConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("TimeSlots");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Start).IsRequired();
        builder.Property(x => x.End).IsRequired();
        builder
            .HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedTimeSlots)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.UpdatedByUser)
            .WithMany(x => x.UpdatedTimeSlots)
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
