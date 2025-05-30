namespace BookMe.Application.Common.Dtos;

public record GetAvailableTimeSlotsDto
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}
