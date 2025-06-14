using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Mappings;

namespace BookMe.Application.Queries;

public class TimeSlotQueries(IRepository<TimeSlot> repository) : ITimeSlotQueries
{
    public async Task<Result<PagedListDto<TimeSlotDto>>> GetAvailableTimeSlotsAsync(
        DateTimeOffset start,
        DateTimeOffset end,
        int pageIndex = 0,
        int pageSize = 10
    )
    {
        var timeSlots = await repository.GetAllPagedAsync(
            query =>
                query
                    .Where(x =>
                        x.Start >= start
                        && x.End <= end
                        && !x.Bookings.Any(x =>
                            x.Status == BookingStatus.Pending || x.Status == BookingStatus.Confirmed
                        )
                    )
                    .OrderBy(x => x.Start),
            pageIndex: pageIndex,
            pageSize: pageSize
        );

        return Result.Success(timeSlots.MapToDto());
    }

    public async Task<Result<PagedListDto<TimeSlotDto>>> GetPagedTimeSlotsAsync(
        DateTimeOffset start,
        DateTimeOffset end,
        int pageIndex = 0,
        int pageSize = 10
    )
    {
        var timeSlots = await repository.GetAllPagedAsync(
            query => query.Where(x => x.Start >= start && x.End <= end).OrderBy(x => x.Start),
            includes: new[] { nameof(TimeSlot.Bookings) },
            pageIndex: pageIndex,
            pageSize: pageSize
        );

        return Result.Success(timeSlots.MapToDto());
    }

    public async Task<Result<GetAvailableDatesResponseDto>> GetAvailableDatesAsync(
        DateTimeOffset start,
        DateTimeOffset end
    )
    {
        var timeSlots = await repository.GetAllWithSelectorAsync(
            x => x.Start.Date,
            query => query.Where(x => x.Start >= start && x.End <= end).OrderBy(x => x.Start)
        );

        var dates = timeSlots.Distinct();

        return Result.Success(new GetAvailableDatesResponseDto { Dates = dates });
    }
}
