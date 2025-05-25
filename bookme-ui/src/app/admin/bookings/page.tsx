"use client";
import CreateTimeSlotForm from "@/_components/admin/CreateTimeSlotForm";
import TimeSlotList from "@/_components/admin/TimeSlotList";
import { withAuth } from "@/_components/auth/AuthGuard";
import { ROLES } from "@/_lib/enums/constant";
import { Calendar } from "@heroui/react";
import {
  DateValue,
  getLocalTimeZone,
  now,
} from "@internationalized/date";
import React from "react";

const BookingAdminPage = () => {
  const [selectedDate, setSelectedDate] = React.useState<DateValue>(
    now(getLocalTimeZone())
  );

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-12 ">Bookings</h1>

      <div className="grid grid-cols-2 gap-4">
        <div>
          <Calendar
            aria-label="Date (Uncontrolled)"
            //@ts-expect-error there seems to be a type issue
            defaultValue={now(getLocalTimeZone())}
            onChange={setSelectedDate}
          />
        </div>
        <div>
          <TimeSlotList
            selectedDate={selectedDate}
          />
          <CreateTimeSlotForm />
        </div>
      </div>
    </div>
  );
};

export default withAuth(BookingAdminPage, ROLES.ADMIN, {
  fallbackPath: "/unauthorized",
});
