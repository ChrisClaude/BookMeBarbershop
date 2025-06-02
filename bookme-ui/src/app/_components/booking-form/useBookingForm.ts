"use client";
import { useAuth } from "@/_hooks/useAuth";
import {
  isNullOrWhiteSpace,
  toE164,
  validatePhoneNumber,
} from "@/_lib/utils/common.utils";
import { DateValue } from "@heroui/react";
import { getLocalTimeZone, now } from "@internationalized/date";
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
    bookingDate: now(getLocalTimeZone()),
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
      isSuccess: isCodeSent,
    },
  ] = useVerifyPhoneNumberMutation();

  const [verifyCodeNumber, { isLoading: isVerifyingCode }] =
    useVerifyCodeNumberMutation();

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
          .then()
          .catch((error) => {
            setErrors([
              {
                field: "phone-number",
                message: JSON.stringify(error),
              },
            ]);
          });
      } else {
        setShowConfirmation(true);
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
      })
      .catch((error) => {
        setErrors([
          {
            field: "verification-code",
            message: JSON.stringify(error),
          },
        ]);
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
