"use client";
import { useAuth } from "@/_hooks/useAuth";
import {
  isNullOrWhiteSpace,
  toE164,
  validatePhoneNumber,
  isNotNullOrUndefined,
} from "@/_lib/utils/common.utils";
import React, { useCallback, useState } from "react";
import { UserDto } from "@/_lib/codegen";
import {
  useVerifyCodeNumberMutation,
  useVerifyPhoneNumberMutation,
} from "@/_lib/queries";
import { E164Number } from "libphonenumber-js";

const useProfileForm = () => {
  const { userProfile } = useAuth();
  const [errors, setErrors] = React.useState<
    { field: string; message: string }[]
  >([]);

  const [formData, setFormData] = React.useState<
    UserDto & {
      verificationCode: string | undefined;
      phoneNumberE164: E164Number | undefined;
    }
  >({
    ...userProfile,
    phoneNumberE164: isNotNullOrUndefined(userProfile?.phoneNumber)
      ? toE164(userProfile?.phoneNumber)
      : undefined,
    verificationCode: undefined,
  });

  const [
    isPhoneNumberVerificationProcess,
    setIsPhoneNumberVerificationProcessing,
  ] = useState(false);

  const [
    verifyPhoneNumber,
    { isLoading: isVerifyingPhoneNumber, isSuccess: isCodeSent },
  ] = useVerifyPhoneNumberMutation();

  const [verifyCodeNumber, { isLoading: isVerifyingCode }] =
    useVerifyCodeNumberMutation();

  const onSubmit = useCallback(
    (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      const phoneErrors = validatePhoneNumber(formData.phoneNumberE164);
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
        // todo: update user profile
      }
    },
    [
      formData.phoneNumber,
      formData.phoneNumberE164,
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
    setFormData,
    onSubmit,
    handleVerifyCode,
  };
};

export default useProfileForm;
