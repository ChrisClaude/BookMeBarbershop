using System;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;

namespace BookMe.Application.Interfaces.Queries;

public interface ITimeSlotQueries
{
    Task<Result<IEnumerable<TimeSlotDto>>> GetAvailableTimeSlotsAsync(DateTimeOffset start, DateTimeOffset end);
}
