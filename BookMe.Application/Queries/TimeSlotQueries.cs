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
    public async Task<Result<IEnumerable<TimeSlotDto>>> GetAvailableTimeSlotsAsync(DateTimeOffset start, DateTimeOffset end)
    {
        var timeSlots = await repository.GetAllAsync(query => query.Where(x => x.Start >= start && x.End <= end && x.Booking == null));
        return Result<IEnumerable<TimeSlotDto>>.Success(timeSlots.Select(x => x.MapToDto()));
    }
}
