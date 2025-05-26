using System;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;

namespace BookMe.Application.Interfaces.Queries;

public interface ITimeSlotQueries
{
    Task<Result<IPagedList<TimeSlotDto>>> GetAvailableTimeSlotsAsync(
        DateTimeOffset start,
        DateTimeOffset end
    );

    Task<Result<IPagedList<TimeSlotDto>>> GetPagedTimeSlotsAsync(
        DateTimeOffset start,
        DateTimeOffset end
    );
}
