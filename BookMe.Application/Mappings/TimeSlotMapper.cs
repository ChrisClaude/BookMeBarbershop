using BookMe.Application.Common.Dtos;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;

namespace BookMe.Application.Mappings;

public static class TimeSlotMapper
{
    public static TimeSlotDto MapToDto(this TimeSlot timeSlot)
    {
        return new TimeSlotDto
        {
            Id = timeSlot.Id,
            Start = timeSlot.Start,
            End = timeSlot.End,
            IsAvailable = timeSlot.Bookings == null
        };
    }

    public static IPagedList<TimeSlotDto> MapToDto(this IPagedList<TimeSlot> timeSlots)
    {
        return new PagedList<TimeSlotDto>(
            timeSlots.Select(x => x.MapToDto()).ToList(),
            timeSlots.PageIndex,
            timeSlots.PageSize,
            timeSlots.TotalCount
        );;
    }
}
