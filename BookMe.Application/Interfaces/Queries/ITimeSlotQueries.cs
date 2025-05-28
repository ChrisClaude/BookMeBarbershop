using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;

namespace BookMe.Application.Interfaces.Queries;

public interface ITimeSlotQueries
{
    Task<Result<IPagedList<TimeSlotDto>>> GetAvailableTimeSlotsAsync(
        DateTimeOffset start,
        DateTimeOffset end,
        int pageNumber = 1,
        int pageSize = 10
    );

    Task<Result<IPagedList<TimeSlotDto>>> GetPagedTimeSlotsAsync(
        DateTimeOffset start,
        DateTimeOffset end,
        int pageNumber = 1,
        int pageSize = 10
    );
}
