"use client";
import React from "react";
import useProfileForm from "./useProfileForm";
import { CgProfile } from "react-icons/cg";
import { Button, Form, Input } from "@heroui/react";
import PhoneInput from "react-phone-number-input";
import PhoneVerificationFormFragment from "../booking-form/PhoneVerificationFragment";
import FormSuccessErrorDisplay from "../FormSuccessErrorDisplay";

const ProfileForm = () => {
  const {
    errors,
    formData,
    userProfile,
    isPhoneNumberVerificationProcess,
    isVerifyingPhoneNumber,
    isCodeSent,
    isVerifyingCode,
    isUpdatingUserProfile,
    isProfileUpdated,
    setFormData,
    onSubmit,
    handleVerifyCode,
    handleInputChange,
  } = useProfileForm();

  return (
    <section className="py-12 md:py-20 lg:py-24 px-4 md:px-8 lg:px-48">
      <h1 className="text-2xl font-bold mb-12 text-center">Profile</h1>

      <div className="flex justify-center mb-12">
        <CgProfile size={80} className="text-primary" aria-hidden="true" />
      </div>

      <Form
        className="w-full flex justify-center items-center space-y-4"
        onSubmit={onSubmit}
      >
        <div className="flex flex-col gap-4 w-full lg:max-w-lg max-w-full px-4 sm:px-0 sm:max-w-md">
          <FormSuccessErrorDisplay
            errors={errors}
            success={isProfileUpdated}
            successMessage="Profile updated successfully"
          />

          <Input
            isRequired
            errorMessage="Please enter a valid email"
            label="Email"
            labelPlacement="outside"
            name="email"
            placeholder="Enter your email"
            id="email"
            type="email"
            value={formData.email || ""}
            isReadOnly
            aria-readonly="true"
          />

          <Input
            isRequired
            errorMessage="Please enter a valid name"
            label="Name"
            labelPlacement="outside"
            name="name"
            placeholder="Enter your name"
            type="text"
            value={formData.name || ""}
            onChange={(e) => handleInputChange("name", e.target.value)}
            aria-invalid={errors.some((e) => e.field === "name")}
          />

          <Input
            isRequired
            errorMessage="Please enter a valid surname"
            label="Surname"
            labelPlacement="outside"
            name="surname"
            placeholder="Enter your surname"
            type="text"
            value={formData.surname || ""}
            onChange={(e) => handleInputChange("surname", e.target.value)}
            aria-invalid={errors.some((e) => e.field === "surname")}
          />

          <div className="flex flex-col gap-2">
            <label htmlFor="phone-input">Phone Number:</label>
            <PhoneInput
              id="phone-input"
              placeholder="Enter phone number"
              value={formData.phoneNumberE164}
              onChange={(value) => {
                setFormData((prevState) => ({
                  ...prevState,
                  phoneNumberE164: value,
                }));
              }}
              defaultCountry="PL"
              aria-invalid={errors.some((e) => e.field === "phone-number")}
            />
            {isCodeSent && (
              <p className="text-sm text-gray-500 text-center">
                We&apos;ll send you a verification code to confirm your number.
              </p>
            )}
            {errors.find((error) => error.field === "phone-number") && (
              <p className="text-red-500" role="alert">
                {
                  errors.find((error) => error.field === "phone-number")
                    ?.message
                }
              </p>
            )}
          </div>

          {isPhoneNumberVerificationProcess && (
            <PhoneVerificationFormFragment
              errors={errors}
              formData={formData}
              updateVerificationCode={(verificationCode) => {
                handleInputChange("verificationCode", verificationCode);
              }}
              handleVerifyCode={handleVerifyCode}
              isVerifyingCode={isVerifyingCode}
            />
          )}

          {!isPhoneNumberVerificationProcess && (
            <Button
              className="w-full"
              color="primary"
              type="submit"
              isLoading={isVerifyingPhoneNumber || isUpdatingUserProfile}
              isDisabled={isVerifyingPhoneNumber || isUpdatingUserProfile}
            >
              {userProfile?.isPhoneNumberVerified
                ? "Save Profile"
                : "Verify Phone Number"}
            </Button>
          )}
        </div>
      </Form>
    </section>
  );
};

export default ProfileForm;
