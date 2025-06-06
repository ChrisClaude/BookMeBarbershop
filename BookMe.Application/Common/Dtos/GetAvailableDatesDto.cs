namespace BookMe.Application.Common.Dtos;

public record GetAvailableDatesDto
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
}

public record GetAvailableDatesResponseDto
{
    public IEnumerable<DateTime> Dates { get; set; }
}
