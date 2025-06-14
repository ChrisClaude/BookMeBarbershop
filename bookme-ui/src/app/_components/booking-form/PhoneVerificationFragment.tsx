import { Button, Input } from "@heroui/react";
import React from "react";

const PhoneVerificationFormFragment = ({
  errors,
  formData,
  updateVerificationCode,
  handleVerifyCode,
  isVerifyingCode,
}: {
  errors: { field: string; message: string }[];
  formData: { verificationCode: string | undefined };
  updateVerificationCode: (verificationCode: string) => void;
  handleVerifyCode: () => void;
  isVerifyingCode: boolean;
}) => {
  return (
    <>
      <div className="flex flex-col gap-2">
        <label htmlFor="verification-code">Verification Code:</label>
        <Input
          id="verification-code"
          type="text"
          placeholder="Enter verification code"
          value={formData.verificationCode || ""}
          onChange={(e) => {
            updateVerificationCode(e.target.value);
          }}
        />
        {errors.find((error) => error.field === "verification-code") && (
          <p className="text-red-500">
            {
              errors.find((error) => error.field === "verification-code")
                ?.message
            }
          </p>
        )}
      </div>
      <div className="separator">
        <span className="px-2 text-gray-500">Verification</span>
      </div>
      <p className="text-sm text-gray-500 text-center">
        We&apos;ve sent a verification code to your phone. Please enter it above
        to verify your number.
      </p>
      <Button
        className="w-full"
        color="primary"
        onPress={handleVerifyCode}
        isLoading={isVerifyingCode}
      >
        Verify code
      </Button>
    </>
  );
};

export default PhoneVerificationFormFragment;
