import { ApiBookingTimeslotsPostRequest } from "@/_lib/codegen";
import { useCreateTimeSlotMutation } from "@/_lib/queries";
import { Button, Checkbox, DatePicker, Form } from "@heroui/react";
import { now, getLocalTimeZone, DateValue } from "@internationalized/date";
import React, { useCallback } from "react";

export type ValidationError = string | string[];
export type ValidationErrors = Record<string, ValidationError>;

const CreateTimeSlotForm = ({ onSuccess }: { onSuccess?: () => void }) => {
  const [errors, setErrors] = React.useState<ValidationErrors>({});
  const [formData, setFormData] = React.useState<{
    startDateTime: DateValue;
    endDateTime: DateValue;
    isAllDay: boolean;
  }>({
    startDateTime: now(getLocalTimeZone()).add({ hours: 1 }),
    endDateTime: now(getLocalTimeZone()).add({ hours: 2 }),
    isAllDay: false,
  });

  const [createTimeSlot, { isLoading, error, isSuccess }] =
    useCreateTimeSlotMutation();

  const onSubmit = useCallback(
    (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();

      // validations
      if (formData.startDateTime > formData.endDateTime) {
        setErrors({
          ...errors,
          startDateTime: "Start date must be before end date",
        });

        return;
      }

      const request: ApiBookingTimeslotsPostRequest = {
        createTimeSlotsDto: {
          startDateTime: formData.startDateTime.toDate(getLocalTimeZone()),
          endDateTime: formData.endDateTime.toDate(getLocalTimeZone()),
          isAllDay: formData.isAllDay,
        },
      };

      createTimeSlot(request)
        .unwrap()
        .then(() => {
          if (onSuccess) {
            onSuccess();
          }
        });
    },
    [
      createTimeSlot,
      errors,
      formData.endDateTime,
      formData.startDateTime,
      formData.isAllDay,
      onSuccess,
    ]
  );

  return (
    <>
      {isSuccess && (
        <div className="p-4 border rounded-lg bg-green-50 text-center">
          <p className="text-green-500">Time slot created successfully</p>
        </div>
      )}

      {error && (
        <div className="p-4 border rounded-lg bg-red-50 text-center">
          <p className="text-red-500">
            Error creating time slot:{" "}
            {JSON.stringify(error)}
          </p>
        </div>
      )}

      <Form
        className="w-full justify-center items-center space-y-4"
        validationErrors={errors}
        onSubmit={onSubmit}
      >
        <div className="flex flex-col gap-4 w-full lg:max-w-lg max-w-md">
          <DatePicker
            isRequired
            id="startDateTime"
            name="startDateTime"
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
            labelPlacement="outside"
            variant="bordered"
            isReadOnly={isLoading}
          />

          <DatePicker
            isRequired
            id="endDateTime"
            name="endDateTime"
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
            labelPlacement="outside"
            variant="bordered"
            isReadOnly={isLoading}
          />

          <Checkbox
            id="isAllDay"
            name="isAllDay"
            isSelected={formData.isAllDay}
            onValueChange={(isSelected) => {
              setFormData({
                ...formData,
                isAllDay: isSelected,
              });
            }}
            isDisabled={isLoading}
          >
            Create time slots for the whole day
          </Checkbox>

          <div className="flex gap-4">
            <Button
              className={`w-full ${isLoading ? "cursor-not-allowed" : ""}`}
              color="primary"
              type="submit"
              isDisabled={isLoading}
            >
              Submit
            </Button>
          </div>
        </div>
      </Form>
    </>
  );
};

export default CreateTimeSlotForm;
