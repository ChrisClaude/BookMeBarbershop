import { ApiBookingTimeslotsPostRequest } from "@/_lib/codegen";
import { useCreateTimeSlotMutation } from "@/_lib/queries";
import { Button, DatePicker, Form } from "@heroui/react";
import { now, getLocalTimeZone, DateValue } from "@internationalized/date";
import React, { useCallback } from "react";

export type ValidationError = string | string[];
export type ValidationErrors = Record<string, ValidationError>;
export interface ValidationResult {
  isInvalid: boolean;
  validationErrors: string[];
  validationDetails: ValidityState;
}

const CreateTimeSlotForm = () => {
  const [errors] = React.useState<ValidationErrors>({});
  const [formData, setFormData] = React.useState<{
    startDateTime: DateValue;
    endDateTime: DateValue;
  }>({
    startDateTime: now(getLocalTimeZone()).add({ hours: 1 }),
    endDateTime: now(getLocalTimeZone()).add({ hours: 2 }),
  });

  const [createTimeSlot] = useCreateTimeSlotMutation();

  const onSubmit = useCallback(
    (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();

      if (!formData) {
        return;
      }

      const request: ApiBookingTimeslotsPostRequest = {
        createTimeSlotsDto: {
          startDateTime: formData.startDateTime.toDate(getLocalTimeZone()),
          endDateTime: formData.endDateTime.toDate(getLocalTimeZone()),
        },
      };

      createTimeSlot(request);
    },
    [createTimeSlot, formData]
  );

  return (
    <>
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
            defaultValue={now(getLocalTimeZone()).add({ hours: 1 })}
            //@ts-expect-error there seems to be a type issue
            value={formData.startDateTime}
            onChange={(value: DateValue | null) => {
              if (!value) {
                return;
              }

              setFormData({
                ...formData,
                startDateTime: value,
              });
            }}
            label="Start date and time"
            variant="bordered"
          />

          <DatePicker
            hideTimeZone
            showMonthAndYearPickers
            //@ts-expect-error there seems to be a type issue
            defaultValue={now(getLocalTimeZone()).add({ hours: 1 })}
            //@ts-expect-error there seems to be a type issue
            value={formData.endDateTime}
            onChange={(value: DateValue | null) => {
              if (!value) {
                return;
              }

              setFormData({
                ...formData,
                endDateTime: value,
              });
            }}
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

export default CreateTimeSlotForm;
