using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;

namespace BookMe.Application.Interfaces.Queries;

public interface ITimeSlotQueries
{
    Task<Result<PagedListDto<TimeSlotDto>>> GetAvailableTimeSlotsAsync(
        DateTimeOffset start,
        DateTimeOffset end,
        int pageNumber = 1,
        int pageSize = 10
    );

    Task<Result<GetAvailableDatesResponseDto>> GetAvailableDatesAsync(
        DateTimeOffset start,
        DateTimeOffset end
    );

    Task<Result<PagedListDto<TimeSlotDto>>> GetPagedTimeSlotsAsync(
        DateTimeOffset start,
        DateTimeOffset end,
        int pageNumber = 1,
        int pageSize = 10
    );
}
