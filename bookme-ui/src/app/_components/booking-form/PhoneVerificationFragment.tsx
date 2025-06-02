import { TimeSlotDto } from "@/_lib/codegen";
import { Button, DateValue } from "@heroui/react";
import { E164Number } from "libphonenumber-js";
import React from "react";

const PhoneVerificationFormFragment = ({
  errors,
  formData,
  setFormData,
  handleVerifyCode,
  isVerifyingCode,
}: {
  errors: { field: string; message: string }[];
  formData: {
    phoneNumber: E164Number | undefined;
    bookingDate: DateValue;
    selectedTimeSlot: TimeSlotDto | undefined;
    verificationCode: string | undefined;
  };
  setFormData: React.Dispatch<
    React.SetStateAction<{
      phoneNumber: E164Number | undefined;
      bookingDate: DateValue;
      selectedTimeSlot: TimeSlotDto | undefined;
      verificationCode: string | undefined;
    }>
  >;
  handleVerifyCode: () => void;
  isVerifyingCode: boolean;
}) => {
  return (
    <>
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
