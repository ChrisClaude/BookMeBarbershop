"use client";
import { withAuth } from "@/_components/auth/AuthGuard";
import { PagedListDtoOfBookingDto } from "@/_lib/codegen";
import {
  BOOKING_STATUS,
  bookingStatusToNumber,
  bookingStatusToString,
  ROLES,
  localLinks,
} from "@/_lib/enums/constant";
import { useGetAllBookingsQuery } from "@/_lib/queries";
import { GetAllBookingQueryType, QueryResult } from "@/_lib/queries/rtk.types";
import { getLocalTimeZone, today } from "@internationalized/date";
import { Button, Card, CardBody, CardFooter, CardHeader, Chip, Divider, Spinner } from "@heroui/react";
import { format } from "date-fns";
import Link from "next/link";
import React, { useMemo } from "react";

const AdminPage = () => {
  const request = useMemo<GetAllBookingQueryType>(() => {
    return {
      getBookingsDto: {
        fromDateTime: today(getLocalTimeZone())
          .toDate(getLocalTimeZone())
          .toISOString(),
        bookingStatus: bookingStatusToNumber(BOOKING_STATUS.CONFIRMED),
      },
      pageIndex: 0,
      pageSize: 7,
    };
  }, []);

  const { data: bookings, isFetching, error } =
    useGetAllBookingsQuery<QueryResult<PagedListDtoOfBookingDto>>(request);

  return (
    <div className="p-6 max-w-6xl mx-auto">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Admin Dashboard</h1>
        <Button
          as={Link}
          href={localLinks.admin.bookings}
          color="primary"
        >
          Manage Availability
        </Button>
      </div>

      <Card className="mb-6">
        <CardHeader className="flex gap-3">
          <div>
            <h2 className="text-xl font-semibold">Upcoming Appointments</h2>
            <p className="text-gray-500">Your next 7 confirmed bookings</p>
          </div>
        </CardHeader>
        <Divider />
        <CardBody>
          {isFetching ? (
            <div className="flex justify-center items-center py-8">
              <Spinner size="lg" />
            </div>
          ) : error ? (
            <div className="p-4 border rounded-lg bg-red-50 text-center">
              <p className="text-red-500">Error loading bookings</p>
            </div>
          ) : !bookings?.items?.length ? (
            <div className="p-8 text-center">
              <p className="text-gray-600">No upcoming bookings found</p>
              <Button
                as={Link}
                href={localLinks.admin.bookings}
                color="primary"
                className="mt-4"
              >
                Create Time Slots
              </Button>
            </div>
          ) : (
            <div className="space-y-4">
              {bookings.items.map((booking) => (
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
                      <div className="mt-2">
                        <p className="text-sm font-medium">Customer: {booking.user?.name} {booking.user?.surname}</p>
                        <p className="text-sm text-gray-500">Email: {booking.user?.email}</p>
                        {booking.user?.phoneNumber && (
                          <p className="text-sm text-gray-500">Phone: {booking.user?.phoneNumber}</p>
                        )}
                      </div>
                    </div>
                    <Chip
                      color="success"
                    >
                      {bookingStatusToString(booking.status)}
                    </Chip>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardBody>
        <CardFooter>
          <Button
            as={Link}
            href={localLinks.admin.users}
            variant="flat"
            color="primary"
            className="ml-auto"
          >
            View All Users
          </Button>
        </CardFooter>
      </Card>
    </div>
  );
};

export default withAuth(AdminPage, ROLES.ADMIN, {
  fallbackPath: "/unauthorized",
});
