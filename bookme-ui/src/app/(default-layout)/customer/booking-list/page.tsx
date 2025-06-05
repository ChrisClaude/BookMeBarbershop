"use client";
import { withAuth } from "@/_components/auth/AuthGuard";
import {
  ApiBookingCancelBookingPostRequest,
  BookingDto,
  PagedListDtoOfBookingDto,
} from "@/_lib/codegen";
import {
  BOOKING_STATUS,
  bookingStatusToNumber,
  bookingStatusToString,
  localLinks,
  ROLES,
} from "@/_lib/enums/constant";
import { useCancelBookingMutation, useGetBookingsQuery } from "@/_lib/queries";
import { GetBookingsQueryType, QueryResult } from "@/_lib/queries/rtk.types";
import { BookingStatusType } from "@/_lib/types/common.types";
import { Button, Chip, Pagination, Spinner, addToast } from "@heroui/react";
import { getLocalTimeZone, today } from "@internationalized/date";
import { format } from "date-fns";
import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import React, { useCallback, useEffect, useMemo } from "react";

const BookingListPage = () => {
  const [pageIndex, setPageIndex] = React.useState(0);
  const [selectedStatus, setSelectedStatus] =
    React.useState<BookingStatusType | null>(null);
  const router = useRouter();
  const searchParams = useSearchParams();
  const [
    cancelBooking,
    { isLoading: isCancellingBooking, isSuccess: isBookingCancelled },
  ] = useCancelBookingMutation();

  const getBookingsRequest = useMemo<GetBookingsQueryType>(() => {
    const fromDateTime = today(getLocalTimeZone()).toDate(getLocalTimeZone());

    return {
      getBookingsDto: {
        fromDateTime: fromDateTime.toISOString(),
        bookingStatus: selectedStatus
          ? bookingStatusToNumber(selectedStatus)
          : null,
      },
      pageIndex,
      pageSize: 10,
    };
  }, [pageIndex, selectedStatus]);

  const {
    data: pagedBookings,
    isFetching,
    error,
  } = useGetBookingsQuery<QueryResult<PagedListDtoOfBookingDto>>(
    getBookingsRequest
  );

  useEffect(() => {
    const page = searchParams.get("page");
    if (page) setPageIndex(parseInt(page) - 1);

    const status = searchParams.get("status") as BookingStatusType;
    if (status) setSelectedStatus(status);
  }, [searchParams]);

  useEffect(() => {
    if (isBookingCancelled) {
      addToast({
        title: "Booking Cancelled",
        description: "Your booking has been cancelled.",
        color: "success",
      });
    }
  }, [isBookingCancelled]);

  const updateQueryParams = React.useCallback(
    (params: { page?: number; status?: string | null }) => {
      const newParams = new URLSearchParams(searchParams);
      if (params.page) newParams.set("page", params.page.toString());
      if (params.status !== undefined) {
        if (params.status === null) newParams.delete("status");
        else newParams.set("status", params.status);
      }
      router.push(`?${newParams.toString()}`);
    },
    [router, searchParams]
  );

  const handlePageChange = (page: number) => {
    updateQueryParams({ page: page + 1 });
    setPageIndex(page);
  };

  const handleStatusChange = useCallback(
    (status: string | null) => {
      updateQueryParams({ status, page: 1 });
      setPageIndex(0);
      setSelectedStatus(status as BookingStatusType);
    },
    [updateQueryParams]
  );

  const handleCancelBooking = useCallback(
    (booking: BookingDto) => {
      const cancellationRequest: ApiBookingCancelBookingPostRequest = {
        cancelBookingDto: {
          bookingId: booking.id,
        },
      };

      const cancelBookingPromise = cancelBooking(cancellationRequest);

      addToast({
        title: "Cancelling Booking",
        description: "Your booking is being cancelled.",
        promise: cancelBookingPromise,
      });
    },
    [cancelBooking]
  );

  return (
    <>
      <div className="p-4 max-w-6xl mx-auto">
        <h1 className="text-2xl font-bold mb-6">My Bookings</h1>

        <div className="mb-6 flex flex-wrap gap-2">
          <Button
            color={selectedStatus === null ? "primary" : "default"}
            variant={selectedStatus === null ? "solid" : "light"}
            onPress={() => handleStatusChange(null)}
          >
            All
          </Button>
          <Button
            color={selectedStatus === "Pending" ? "primary" : "default"}
            variant={selectedStatus === "Pending" ? "solid" : "light"}
            onPress={() => handleStatusChange("Pending")}
          >
            Pending
          </Button>
          <Button
            color={selectedStatus === "Confirmed" ? "primary" : "default"}
            variant={selectedStatus === "Confirmed" ? "solid" : "light"}
            onPress={() => handleStatusChange("Confirmed")}
          >
            Confirmed
          </Button>
          <Button
            color={selectedStatus === "Completed" ? "primary" : "default"}
            variant={selectedStatus === "Completed" ? "solid" : "light"}
            onPress={() => handleStatusChange("Completed")}
          >
            Completed
          </Button>
          <Button
            color={selectedStatus === "Cancelled" ? "primary" : "default"}
            variant={selectedStatus === "Cancelled" ? "solid" : "light"}
            onPress={() => handleStatusChange("Cancelled")}
          >
            Cancelled
          </Button>
        </div>

        {isFetching ? (
          <div className="p-8 text-center">
            <Spinner size="lg" />
            <p className="mt-2 text-gray-600">Loading your bookings...</p>
          </div>
        ) : error ? (
          <div className="p-4 border rounded-lg bg-red-50 text-center">
            <p className="text-red-500">Error loading bookings</p>
          </div>
        ) : !pagedBookings?.items?.length ? (
          <div className="p-8 border rounded-lg bg-gray-50 text-center">
            <p className="text-gray-600">No bookings found</p>
            <Button
              as={Link}
              href={localLinks.customer.bookingAppointment}
              color="primary"
              className="mt-4"
            >
              Book an Appointment
            </Button>
          </div>
        ) : (
          <>
            <div className="grid gap-4">
              {pagedBookings.items.map((booking) => (
                <div
                  key={booking.id}
                  className="border rounded-lg p-4 shadow-sm hover:shadow-md transition-shadow"
                >
                  <div className="flex justify-between items-start">
                    <div>
                      <h3 className="font-semibold text-lg">
                        {format(
                          new Date(booking.timeSlot?.start || ""),
                          "EEEE, MMMM d, yyyy"
                        )}
                      </h3>
                      <p className="text-gray-600">
                        {format(
                          new Date(booking.timeSlot?.start || ""),
                          "h:mm a"
                        )}{" "}
                        -
                        {format(
                          new Date(booking.timeSlot?.end || ""),
                          "h:mm a"
                        )}
                      </p>
                    </div>
                    <Chip
                      color={
                        booking.status ===
                        bookingStatusToNumber(BOOKING_STATUS.PENDING)
                          ? "warning"
                          : booking.status ===
                            bookingStatusToNumber(BOOKING_STATUS.CONFIRMED)
                          ? "success"
                          : booking.status ===
                            bookingStatusToNumber(BOOKING_STATUS.COMPLETED)
                          ? "primary"
                          : "danger"
                      }
                    >
                      {bookingStatusToString(booking.status)}
                    </Chip>
                  </div>

                  <div className="mt-4 flex justify-end gap-2">
                    {booking.status ===
                      bookingStatusToNumber(BOOKING_STATUS.PENDING) && (
                      <Button
                        color="danger"
                        size="sm"
                        variant="light"
                        onPress={() => handleCancelBooking(booking)}
                        isDisabled={isCancellingBooking}
                      >
                        Cancel
                      </Button>
                    )}
                    <Button
                      as={Link}
                      href={localLinks.customer.bookingAppointment}
                      color="primary"
                      size="sm"
                      isDisabled={isCancellingBooking}
                    >
                      Book Another
                    </Button>
                  </div>
                </div>
              ))}
            </div>

            {pagedBookings.totalPages && pagedBookings.totalPages > 1 && (
              <div className="mt-6 flex justify-center">
                <Pagination
                  total={pagedBookings.totalPages}
                  initialPage={pageIndex}
                  page={pageIndex + 1}
                  onChange={(page) => handlePageChange(page - 1)}
                />
              </div>
            )}
          </>
        )}
      </div>
    </>
  );
};

export default withAuth(BookingListPage, ROLES.CUSTOMER, {
  fallbackPath: "/unauthorized",
});
