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
            IsAvailable = timeSlot.Bookings == null,
            AllowAutoConfirmation = timeSlot.AllowAutoConfirmation
        };
    }

    public static PagedListDto<TimeSlotDto> MapToDto(this IPagedList<TimeSlot> timeSlots)
    {
        return new PagedListDto<TimeSlotDto>
        {
            PageIndex = timeSlots.PageIndex,
            PageSize = timeSlots.PageSize,
            TotalCount = timeSlots.TotalCount,
            TotalPages = timeSlots.TotalPages,
            HasPreviousPage = timeSlots.HasPreviousPage,
            HasNextPage = timeSlots.HasNextPage,
            Items = timeSlots.Select(x => x.MapToDto()).ToList()
        };
    }
}
