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
        Guid userId,
        DateTime fromDateTime,
        BookingStatus? bookingStatus = null,
        int pageIndex = 0,
        int pageSize = 10
    )
    {
        var bookings = await repository.GetAllPagedAsync(
            queryable =>
                queryable
                    .Where(x => x.User.Id == userId)
                    .Where(x => x.TimeSlot.Start >= fromDateTime)
                    .Where(x => bookingStatus == null || x.Status == bookingStatus)
                    .OrderBy(x =>
                        x.Status == BookingStatus.Confirmed || x.Status == BookingStatus.Pending
                            ? 0
                            : 1
                    )
                    .ThenBy(x => x.TimeSlot.Start),
            includes: new[] { nameof(Booking.User), nameof(Booking.TimeSlot) },
            pageIndex: pageIndex,
            pageSize: pageSize
        );

        return Result.Success(bookings.MapToDto());
    }
}
