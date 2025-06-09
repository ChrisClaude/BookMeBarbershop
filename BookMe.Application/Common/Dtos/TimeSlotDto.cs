namespace BookMe.Application.Common.Dtos;

public record TimeSlotDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool IsAvailable { get; set; }

    // TODO: We should have a different dto for admin and customer
    // for instance for customer we don't need to show AllowAutoConfirmation
    public bool? AllowAutoConfirmation { get; set; }
}
