using System;
using BookMe.Application.Common;
using BookMe.Application.Common.Dtos;
using BookMe.Application.Common.Dtos.Bookings;
using BookMe.Application.Entities;

namespace BookMe.Application.Interfaces.Queries;

public interface IBookingQueries
{
    Task<Result<PagedListDto<BookingDto>>> GetPagedBookingsAsync(
        DateTime fromDateTime,
        BookingStatus? bookingStatus = null,
        int pageIndex = 0,
        int pageSize = 10
    );
}
