"use client";
import { useAuth } from "@/_hooks/useAuth";
import { toE164, validatePhoneNumber } from "@/_lib/utils/common.utils";
import { Button, DatePicker, DateValue, Form } from "@heroui/react";
import { getLocalTimeZone, today } from "@internationalized/date";
import { E164Number } from "libphonenumber-js";
import React, { useCallback } from "react";
import PhoneInput from "react-phone-number-input";
import TimeSlotListSlider from "./customer/TimeSlotListSlider";

const BookingForm = () => {
  const { userProfile } = useAuth();
  const [errors, setErrors] = React.useState<
    { field: string; message: string }[]
  >([]);

  const [formData, setFormData] = React.useState<{
    phoneNumber: E164Number | undefined;
    bookingDate: DateValue;
  }>({
    phoneNumber: toE164(userProfile?.phoneNumber ?? ""),
    bookingDate: today(getLocalTimeZone()),
  });

  const onSubmit = useCallback(
    (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      const phoneErrors = validatePhoneNumber(formData.phoneNumber);
      if (phoneErrors.length > 0) {
        setErrors(
          phoneErrors.map((error) => ({
            field: "phone-number",
            message: error,
          }))
        );
        return;
      } else {
        setErrors((prevErrors) =>
          prevErrors.filter((error) => error.field !== "phone-number")
        );
      }
    },
    [formData.phoneNumber]
  );

  return (
    <>
      <h1 className="text-2xl font-bold mb-12 text-center">
        Book your appointment
      </h1>
      <Form
        className="w-full justify-center items-center space-y-4"
        onSubmit={onSubmit}
      >
        <div className="flex flex-col gap-4 w-full lg:max-w-lg max-w-md">
          {errors.length > 0 && (
            <div className="p-4 border rounded-lg bg-red-50 text-center">
              {errors.map((error, index) => (
                <p className="text-red-500" key={index}>
                  {error.field} - {error.message}
                </p>
              ))}
            </div>
          )}

          <div className="flex flex-col gap-2">
            <label htmlFor="phone-input">Phone Number:</label>
            <PhoneInput
              id="phone-input"
              placeholder="Enter phone number"
              value={formData.phoneNumber}
              onChange={(value) => {
                setFormData((prevState) => {
                  return {
                    ...prevState,
                    phoneNumber: value,
                  };
                });
              }}
              defaultCountry="PL"
            />
            {errors.find((error) => error.field === "phone-number") && (
              <p className="text-red-500">
                {
                  errors.find((error) => error.field === "phone-number")
                    ?.message
                }
              </p>
            )}
          </div>

          <DatePicker
            aria-label="Date (Min Date Value)"
            //@ts-expect-error there seems to be a type issue
            defaultValue={today(getLocalTimeZone())}
            minValue={today(getLocalTimeZone())}
            label="Choose a date to see available time slots"
            labelPlacement="outside"
            name="bookingDate"
            onChange={(value: DateValue | null) => {
              if (!value) {
                return;
              }

              setFormData({
                ...formData,
                bookingDate: value,
              });
            }}
          />

          <TimeSlotListSlider selectedDate={formData.bookingDate} />

          <Button className="w-full" color="primary" type="submit">
            Submit
          </Button>
        </div>
      </Form>
    </>
  );
};

export default BookingForm;
