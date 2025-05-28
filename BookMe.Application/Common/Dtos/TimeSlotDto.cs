namespace BookMe.Application.Common.Dtos;

public record TimeSlotDto
{
    public Guid Id { get; set; }
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool IsAvailable { get; set; }
}
