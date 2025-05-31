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
          sendCodeRequest: {
            phoneNumber: formData.phoneNumber,
          },
        })
          .unwrap()
          .then(() => {
            // Set a message to the user that the verification code has been sent
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
      verifyCodeRequest: {
        phoneNumber: formData.phoneNumber,
        code: formData.verificationCode,
      },
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
              <div className="flex flex-col gap-2">
                <label htmlFor="verification-code">Verification Code:</label>
                <input
                  id="verification-code"
                  type="text"
                  className="w-full font-normal bg-transparent outline-none bg-gray-100 p-2 rounded-md hover:bg-gray-200 transition-colors"
                  placeholder="Enter verification code"
                  value={formData.verificationCode || ""}
                  onChange={(e) => {
                    setFormData((prevState) => ({
                      ...prevState,
                      verificationCode: e.target.value,
                    }));
                  }}
                />
                {errors.find(
                  (error) => error.field === "verification-code"
                ) && (
                  <p className="text-red-500">
                    {
                      errors.find(
                        (error) => error.field === "verification-code"
                      )?.message
                    }
                  </p>
                )}
              </div>
              <div className="separator">
                <span className="px-2 text-gray-500">Verification</span>
              </div>
              <p className="text-sm text-gray-500 text-center">
                We&apos;ve sent a verification code to your phone. Please enter
                it above to verify your number.
              </p>
              <Button
                className="w-full"
                color="primary"
                onPress={handleVerifyCode}
                isLoading={isVerifyingCode}
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

              <Button
                className="w-full"
                color="primary"
                type="submit"
                isLoading={isVerifyingPhoneNumber}
                isDisabled={!formData.selectedTimeSlot}
              >
                {userProfile?.isPhoneNumberVerified
                  ? "Book Appointment"
                  : "Verify Phone Number"}
              </Button>
              {!formData.selectedTimeSlot && (
                <p className="text-sm text-center text-gray-500">
                  Please select a time slot to continue
                </p>
              )}
            </Fragment>
          )}
        </div>
      </Form>
    </>
  );
};

export default BookingForm;
