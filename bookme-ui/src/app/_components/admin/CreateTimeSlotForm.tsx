import { ApiBookingTimeslotsPostRequest } from "@/_lib/codegen";
import { BookingService } from "@/_lib/services/booking.service";
import { Button, DatePicker, Form } from "@heroui/react";
import { now, getLocalTimeZone } from "@internationalized/date";
import React, { useCallback } from "react";

export type ValidationError = string | string[];
export type ValidationErrors = Record<string, ValidationError>;
export interface ValidationResult {
  isInvalid: boolean;
  validationErrors: string[];
  validationDetails: ValidityState;
}

const BookingForm = () => {
  const [errors] = React.useState<ValidationErrors>({});
  const [formData] = React.useState<{
    startDateTime: Date;
    endDateTime: Date;
  }>({
    startDateTime: new Date(),
    endDateTime: new Date(),
  });

  const onSubmit = useCallback(
    (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      // const data = Object.fromEntries(new FormData(e.currentTarget));

      if (!formData) {
        return;
      }

      const request: ApiBookingTimeslotsPostRequest = {
        createTimeSlotsDto: {
          startDateTime: formData.startDateTime,
          endDateTime: formData.endDateTime,
        },
      };

      BookingService.createTimeSlot({ request });
    },
    [formData]
  );

  return (
    <>
      <h1 className="text-2xl font-bold mb-12 text-center">
        Book your appointment
      </h1>
      <Form
        className="w-full justify-center items-center space-y-4"
        validationErrors={errors}
        onSubmit={onSubmit}
      >
        <div className="flex flex-col gap-4 w-full lg:max-w-lg max-w-md">
          <DatePicker
            hideTimeZone
            showMonthAndYearPickers
            //@ts-expect-error there seems to be a type issue
            defaultValue={now(getLocalTimeZone())}
            label="Start date and time"
            variant="bordered"
          />

          <DatePicker
            hideTimeZone
            showMonthAndYearPickers
            //@ts-expect-error there seems to be a type issue
            defaultValue={now(getLocalTimeZone())}
            label="End date and time"
            variant="bordered"
          />

          {errors.terms && (
            <span className="text-danger text-small">{errors.terms}</span>
          )}

          <div className="flex gap-4">
            <Button className="w-full" color="primary" type="submit">
              Submit
            </Button>
          </div>
        </div>
      </Form>
    </>
  );
};

export default BookingForm;
