"use client";
import { useAuth } from "@/_hooks/useAuth";
import {
  isNullOrWhiteSpace,
  toE164,
  validatePhoneNumber,
  isNotNullOrUndefined,
} from "@/_lib/utils/common.utils";
import React, { useCallback, useState } from "react";
import { ApiUserProfilePutRequest, UserDto } from "@/_lib/codegen";
import {
  useUpdateUserProfileMutation,
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

  const [
    updateUserProfile,
    {
      isLoading: isUpdatingUserProfile,
      isSuccess: isProfileUpdated,
      error: errorUpdatingProfile,
    },
  ] = useUpdateUserProfileMutation();

  const validateFormDate = (
    formData: UserDto & {
      verificationCode: string | undefined;
      phoneNumberE164: E164Number | undefined;
    },
    setErrors: React.Dispatch<
      React.SetStateAction<{ field: string; message: string }[]>
    >
  ) => {
    const phoneErrors = validatePhoneNumber(formData.phoneNumberE164);
    let isFormValid = true;
    if (phoneErrors.length > 0) {
      setErrors(
        phoneErrors.map((error) => ({
          field: "phone-number",
          message: error,
        }))
      );
      isFormValid = isFormValid && false;
    } else {
      setErrors((prevErrors) =>
        prevErrors.filter((error) => error.field !== "phone-number")
      );

      isFormValid = isFormValid && true;
    }

    if (isNullOrWhiteSpace(formData.name)) {
      setErrors([
        {
          field: "name",
          message: "Name is required",
        },
      ]);
      isFormValid = isFormValid && false;
    } else {
      setErrors((prevErrors) =>
        prevErrors.filter((error) => error.field !== "name")
      );
      isFormValid = isFormValid && true;
    }

    if (isNullOrWhiteSpace(formData.surname)) {
      setErrors([
        {
          field: "surname",
          message: "Surname is required",
        },
      ]);
      isFormValid = isFormValid && false;
    } else {
      setErrors((prevErrors) =>
        prevErrors.filter((error) => error.field !== "surname")
      );

      isFormValid = isFormValid && true;
    }
    return isFormValid;
  };

  const onSubmit = useCallback(
    (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      const isFormValid = validateFormDate(formData, setErrors);

      if (!isFormValid) {
        return;
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
        const updateRequest: ApiUserProfilePutRequest = {
          userUpdateDto: {
            name: formData.name,
            surname: formData.surname,
          },
        };
        updateUserProfile(updateRequest)
          .unwrap()
          .then()
          .catch((error) => {
            setErrors([
              {
                field: "profile-update",
                message: JSON.stringify(error),
              },
            ]);
          });
      }
    },
    [
      formData,
      updateUserProfile,
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
    isUpdatingUserProfile,
    isProfileUpdated,
    errorUpdatingProfile,
    setFormData,
    onSubmit,
    handleVerifyCode,
  };
};

export default useProfileForm;
