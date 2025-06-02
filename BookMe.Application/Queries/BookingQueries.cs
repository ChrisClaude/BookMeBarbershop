using System;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Entities;
using BookMe.Application.Interfaces;
using BookMe.Application.Interfaces.Queries;
using BookMe.Application.Mappings;

namespace BookMe.Application.Queries;

public class BookingQueries(IRepository<Booking> repository) : IBookingQueries
{
    public async Task<Result<PagedListDto<BookingDto>>> GetPagedBookingsAsync(
        DateTime fromDateTime,
        BookingStatus? bookingStatus = null,
        int pageIndex = 0,
        int pageSize = 10
    )
    {
        IPagedList<Booking> bookings;
        if (bookingStatus == null)
        {
            bookings = await repository.GetAllPagedAsync(query => query.Where(x => x.TimeSlot.Start >= fromDateTime), pageIndex, pageSize);

            return Result.Success(bookings.MapToDto());
        }

        bookings = await repository.GetAllPagedAsync(query => query.Where(x => x.TimeSlot.Start >= fromDateTime && x.Status == bookingStatus), pageIndex, pageSize);

        return Result.Success(bookings.MapToDto());
    }
}
