import { ApiBookingTimeslotsPostRequest } from "@/_lib/codegen";
import { useCreateTimeSlotMutation } from "@/_lib/queries";
import { ValidationErrors } from "@/_lib/types/common.types";
import { Button, Checkbox, DatePicker, Form } from "@heroui/react";
import {
  now,
  getLocalTimeZone,
  DateValue,
  toCalendarDateTime,
} from "@internationalized/date";
import React, { useCallback, useEffect } from "react";

const CreateTimeSlotForm = ({
  selectedDate,
  onSuccess,
}: {
  selectedDate: DateValue;
  onSuccess?: () => void;
}) => {
  const [errors, setErrors] = React.useState<ValidationErrors>({});
  const [formData, setFormData] = React.useState<{
    startDateTime: DateValue;
    endDateTime: DateValue;
    isAllDay: boolean;
    autoConfirmation: boolean;
  }>({
    startDateTime: toCalendarDateTime(
      selectedDate,
      now(getLocalTimeZone()).add({ hours: 1 })
    ),
    endDateTime: toCalendarDateTime(
      selectedDate,
      now(getLocalTimeZone()).add({ hours: 2 })
    ),
    isAllDay: false,
    autoConfirmation: true,
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
          allowAutoConfirmation: formData.autoConfirmation,
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
    [createTimeSlot, errors, formData, onSuccess]
  );

  useEffect(() => {
    if (formData.isAllDay) {
      setFormData((prevState) => {
        return {
          ...prevState,
          startDateTime: prevState.startDateTime.set({
            hour: 9,
            minute: 0,
            second: 0,
          }),
          endDateTime: prevState.endDateTime.set({
            hour: 21,
            minute: 59,
            second: 59,
          }),
        };
      });
    }
  }, [formData.isAllDay]);

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
            Error creating time slot: {JSON.stringify(error)}
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
            id="auto-confirmation"
            name="autoConfirmation"
            isSelected={formData.autoConfirmation}
            onValueChange={(isSelected) => {
              setFormData({
                ...formData,
                autoConfirmation: isSelected,
              });
            }}
            isDisabled={isLoading}
          >
            Allow auto confirmation
          </Checkbox>

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
