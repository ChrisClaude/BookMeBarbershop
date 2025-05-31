using System.ComponentModel.DataAnnotations;

namespace BookMe.Application.Entities;

public class TimeSlot : BaseEntity, IAuditable
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool? AllowAutoConfirmation { get; set; }

    // A time slot can have multiple bookings
    // however only one booking can be pending or confirmed at a time
    public IEnumerable<Booking> Bookings { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public User CreatedByUser { get; set; }
    public User UpdatedByUser { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } // For concurrent actions

    public bool IsAvailable =>
        Bookings == null
        || !Bookings.Any(b => b is { Status: BookingStatus.Pending or BookingStatus.Confirmed });
}
