"use client";
import { useAuth } from "@/_hooks/useAuth";
import {
  isNullOrWhiteSpace,
  toE164,
  validatePhoneNumber,
} from "@/_lib/utils/common.utils";
import { Button, DatePicker, DateValue, Form } from "@heroui/react";
import { getLocalTimeZone, today } from "@internationalized/date";
import { E164Number } from "libphonenumber-js";
import React, { Fragment, useCallback, useState } from "react";
import PhoneInput from "react-phone-number-input";
import TimeSlotListSlider from "./customer/TimeSlotListSlider";
import { TimeSlotDto } from "@/_lib/codegen";
import {
  useVerifyCodeNumberMutation,
  useVerifyPhoneNumberMutation,
} from "@/_lib/queries";

const BookingForm = () => {
  const { userProfile } = useAuth();
  const [errors, setErrors] = React.useState<
    { field: string; message: string }[]
  >([]);

  const [formData, setFormData] = React.useState<{
    phoneNumber: E164Number | undefined;
    bookingDate: DateValue;
    selectedTimeSlot: TimeSlotDto | undefined;
    verificationCode: string | undefined;
  }>({
    phoneNumber: toE164(userProfile?.phoneNumber ?? ""),
    bookingDate: today(getLocalTimeZone()),
    selectedTimeSlot: undefined,
    verificationCode: undefined,
  });

  const [
    isPhoneNumberVerificationProcess,
    setIsPhoneNumberVerificationProcessing,
  ] = useState(false);

  const [verifyPhoneNumber, { isLoading: isVerifyingPhoneNumber }] =
    useVerifyPhoneNumberMutation();

  const [verifyCodeNumber, { isLoading: isVerifyingCode }] =
    useVerifyCodeNumberMutation();

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

      if (!userProfile?.isPhoneNumberVerified) {
        setIsPhoneNumberVerificationProcessing(true);
        verifyPhoneNumber({
          phoneNumber: formData.phoneNumber,
        })
          .unwrap()
          .then(() => {
            setIsPhoneNumberVerificationProcessing(false);
          });
      } else {
      }
    },
    [
      formData.phoneNumber,
      userProfile?.isPhoneNumberVerified,
      verifyPhoneNumber,
    ]
  );

  const handleVerifyCode = () => {
    if (isNullOrWhiteSpace(formData.verificationCode)) {
      setErrors([
        {
          field: "verification-code",
          message: "Verification code is required",
        },
      ]);
      return;
    }

    verifyCodeNumber({
      phoneNumber: formData.phoneNumber,
      code: formData.verificationCode,
    })
      .unwrap()
      .then(() => {
        setIsPhoneNumberVerificationProcessing(false);
      });
  };

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
          {isPhoneNumberVerificationProcess && (
            <Fragment>
{/* Add verification code input */}
              <Button
                className="w-full"
                color="primary"
                onPress={handleVerifyCode}
              >
                Verify code
              </Button>
            </Fragment>
          )}

          {!isPhoneNumberVerificationProcess && (
            <Fragment>
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

              <TimeSlotListSlider
                selectedDate={formData.bookingDate}
                onSelectTimeSlot={(timeSlot) => {
                  setFormData({
                    ...formData,
                    selectedTimeSlot: timeSlot,
                  });
                }}
                selectedTimeSlot={formData.selectedTimeSlot}
              />

              <Button className="w-full" color="primary" type="submit">
                Submit
              </Button>
            </Fragment>
          )}
        </div>
      </Form>
    </>
  );
};

export default BookingForm;
