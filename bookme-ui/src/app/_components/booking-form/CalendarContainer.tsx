import { Calendar } from "@heroui/react";
import { DateValue, getLocalTimeZone, today } from "@internationalized/date";
import React from "react";
import useBookingForm from "./useBookingForm";

const CalendarContainer = () => {
  const {
    formData,
    setFormData,
    mappedAvailableDates,
    canShowCalendar,
    isFetchingAvailableDates,
    errorFetchingAvailableDates,
  } = useBookingForm();

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
      onFocusChange={(value: DateValue | null) => {
        if (!value) {
          return;
        }

        setFormData({
          ...formData,
          focusedDate: value,
        });
      }}
      isDateUnavailable={(date) => {
        return mappedAvailableDates.some((x) => x.compare(date) === 0);
      }}
      onChange={(value: DateValue | null) => {
        if (!value) {
          return;
        }

        setFormData({
          ...formData,
          bookingDate: value,
        });
      }}
      visibleMonths={2}
    />
  );
};

export default CalendarContainer;
