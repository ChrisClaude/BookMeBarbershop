"use client";
import { useAuth } from "@/_hooks/useAuth";
import {
  isNullOrWhiteSpace,
  toE164,
  validatePhoneNumber,
} from "@/_lib/utils/common.utils";
import { DateValue } from "@heroui/react";
import { getLocalTimeZone, today } from "@internationalized/date";
import { E164Number } from "libphonenumber-js";
import React, { useCallback, useState } from "react";
import { TimeSlotDto } from "@/_lib/codegen";
import {
  useCreateBookingMutation,
  useVerifyCodeNumberMutation,
  useVerifyPhoneNumberMutation,
} from "@/_lib/queries";

const useBookingForm = () => {
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

  const [
    verifyPhoneNumber,
    {
      isLoading: isVerifyingPhoneNumber,
      error: verifyPhoneNumberError,
      isSuccess: isCodeSent,
    },
  ] = useVerifyPhoneNumberMutation();

  const [
    verifyCodeNumber,
    {
      isLoading: isVerifyingCode,
      isSuccess: isCodeVerified,
      error: verifyCodeError,
    },
  ] = useVerifyCodeNumberMutation();

  const [createBooking, { isLoading: isCreatingBooking }] =
    useCreateBookingMutation();

  const [bookingSuccess, setBookingSuccess] = useState(false);
  const [showConfirmation, setShowConfirmation] = useState(false);

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
            if (verifyPhoneNumberError) {
              setErrors([
                {
                  field: "phone-number",
                  message: verifyPhoneNumberError.toString(),
                },
              ]);
            }
          });
      } else {
        setShowConfirmation(true);
      }
    },
    [
      formData.phoneNumber,
      userProfile?.isPhoneNumberVerified,
      verifyPhoneNumber,
      verifyPhoneNumberError,
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
        if (verifyCodeError) {
          setErrors([
            {
              field: "verification-code",
              message: verifyCodeError.toString(),
            },
          ]);
        }

        if (isCodeVerified) {
          setIsPhoneNumberVerificationProcessing(false);
        }
      });
  };

  return {
    errors,
    formData,
    userProfile,
    isPhoneNumberVerificationProcess,
    isVerifyingPhoneNumber,
    isCodeSent,
    isVerifyingCode,
    isCreatingBooking,
    bookingSuccess,
    showConfirmation,
    setFormData,
    onSubmit,
    handleVerifyCode,
    createBooking,
    setBookingSuccess,
    setShowConfirmation,
  };
};

export default useBookingForm;
