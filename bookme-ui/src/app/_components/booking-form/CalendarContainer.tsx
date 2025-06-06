import { Calendar } from "@heroui/react";
import { DateValue, getLocalTimeZone, today } from "@internationalized/date";
import React from "react";
import { binarySearch } from "@/_lib/utils/common.utils";

const CalendarContainer = ({
  mappedAvailableDates,
  updateBookingDate,
  canShowCalendar,
  isFetchingAvailableDates,
  errorFetchingAvailableDates,
}: {
  mappedAvailableDates: DateValue[];
  updateBookingDate: (date: DateValue) => void;
  canShowCalendar: boolean;
  isFetchingAvailableDates: boolean;
  errorFetchingAvailableDates: string[] | undefined;
}) => {

  if (errorFetchingAvailableDates) {
    return (
      <div className="p-4 border rounded-lg bg-red-50 text-center">
        <p className="text-red-500">Error loading available dates</p>
      </div>
    );
  }

  if (!canShowCalendar || isFetchingAvailableDates) {
    return null;
  }

  return (
    <Calendar
      aria-label="appointment-date"
      //@ts-expect-error there seems to be a type issue
      defaultValue={today(getLocalTimeZone())}
      minValue={today(getLocalTimeZone())}
      label="Choose a date to see available time slots"
      labelPlacement="outside"
      name="bookingDate"
      isDateUnavailable={(date) => {
        return !binarySearch(mappedAvailableDates, date);
      }}
      onChange={(value: DateValue | null) => {
        if (!value) {
          return;
        }

        updateBookingDate(value);
      }}
      visibleMonths={2}
    />
  );
};

export default CalendarContainer;
