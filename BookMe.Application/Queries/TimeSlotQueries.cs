using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Mappings;

namespace BookMe.Application.Queries;

public class TimeSlotQueries(IRepository<TimeSlot> repository) : ITimeSlotQueries
{
    public async Task<Result<PagedListDto<TimeSlotDto>>> GetAvailableTimeSlotsAsync(DateTimeOffset start, DateTimeOffset end, int pageIndex = 0, int pageSize = 10)
    {
        var timeSlots = await repository.GetAllPagedAsync(query => query.Where(x => x.Start >= start && x.End <= end && x.IsAvailable == true), pageIndex, pageSize);

        return Result<PagedListDto<TimeSlotDto>>.Success(timeSlots.MapToDto());
    }

    public async Task<Result<PagedListDto<TimeSlotDto>>> GetPagedTimeSlotsAsync(DateTimeOffset start, DateTimeOffset end, int pageIndex = 0, int pageSize = 10)
    {
        var timeSlots = await repository.GetAllPagedAsync(query => query.Where(x => x.Start >= start && x.End <= end), pageIndex, pageSize);

        return Result<PagedListDto<TimeSlotDto>>.Success(timeSlots.MapToDto());
    }
}
