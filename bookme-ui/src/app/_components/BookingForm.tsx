"use client";
import { useAuth } from "@/_hooks/useAuth";
import { ValidationErrors } from "@/_lib/types/common.types";
import { Button, DateInput, DateValue, Form, Input } from "@heroui/react";
import { getLocalTimeZone, today } from "@internationalized/date";
import React, { useCallback, useMemo } from "react";

const BookingForm = () => {
  const { userProfile } = useAuth();
  const [errors, setErrors] = React.useState<string[]>([]);

  const [formData, setFormData] = React.useState<{
    phoneNumber: string;
    bookingDate: DateValue;
  }>({
    phoneNumber: userProfile?.phoneNumber ?? "",
    bookingDate: today(getLocalTimeZone()),
  });

  const canSubmit = useMemo(() => false, []);

  const onSubmit = useCallback(
    (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      if (!canSubmit) {
        setErrors(["Please fix the errors before submitting"]);
        return;
      }
    },
    [canSubmit]
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
                  {error}
                </p>
              ))}
            </div>
          )}

          <Input
            isRequired
            label="Phone number"
            labelPlacement="outside"
            name="phoneNumber"
            placeholder="Enter your email"
            type="tel"
          />

          <DateInput
            // @ts-expect-error there seems to be a type issue
            defaultValue={today(getLocalTimeZone())}
            isRequired
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

          <Button className="w-full" color="primary" type="submit">
            Submit
          </Button>
        </div>
      </Form>
    </>
  );
};

export default BookingForm;
