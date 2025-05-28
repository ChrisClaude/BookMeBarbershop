namespace BookMe.Application.Common.Dtos;

public record GetTimeSlotsDto
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public bool IsAvailable { get; set; }
}
