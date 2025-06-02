using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;

namespace BookMe.Application.Mappings;

public static class BookingMapper
{
    public static BookingDto MapToDto(this Booking booking)
    {
        return new BookingDto
        {
            Id = booking.Id,
            Status = booking.Status,
            User = booking.User.MapToDto(),
            TimeSlot = booking.TimeSlot.MapToDto()
        };
    }

    public static PagedListDto<BookingDto> MapToDto(this IPagedList<Booking> bookings)
    {
        return new PagedListDto<BookingDto>
        {
            PageIndex = bookings.PageIndex,
            PageSize = bookings.PageSize,
            TotalCount = bookings.TotalCount,
            TotalPages = bookings.TotalPages,
            HasPreviousPage = bookings.HasPreviousPage,
            HasNextPage = bookings.HasNextPage,
            Items = bookings.Select(x => x.MapToDto()).ToList()
        };
    }
}
