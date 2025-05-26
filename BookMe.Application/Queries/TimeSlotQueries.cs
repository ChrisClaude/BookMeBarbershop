using System;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Mappings;

namespace BookMe.Application.Queries;

public class TimeSlotQueries(IRepository<TimeSlot> repository) : ITimeSlotQueries
{
    public async Task<Result<IPagedList<TimeSlotDto>>> GetAvailableTimeSlotsAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var timeSlots = await repository.GetAllPagedAsync(query => query.Where(x => x.Start >= start && x.End <= end && x.IsAvailable));
        return Result<IPagedList<TimeSlotDto>>.Success(timeSlots.MapToDto());
    }

    public async Task<Result<IPagedList<TimeSlotDto>>> GetPagedTimeSlotsAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var timeSlots = await repository.GetAllPagedAsync(query => query.Where(x => x.Start >= start && x.End <= end));
        return Result<IPagedList<TimeSlotDto>>.Success(timeSlots.MapToDto());
    }
}
