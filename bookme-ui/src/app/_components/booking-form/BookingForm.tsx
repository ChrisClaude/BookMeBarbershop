"use client";
import React from "react";
import { Button, Form } from "@heroui/react";
import PhoneInput from "react-phone-number-input";
import TimeSlotListSlider from "../customer/TimeSlotListSlider";
import useBookingForm from "./useBookingForm";
import BookingConfirmationDialog from "./BookingConfirmationDialog";
import PhoneVerificationFormFragment from "./PhoneVerificationFragment";
import CalendarContainer from "./CalendarContainer";
import FormSuccessErrorDisplay from "../FormSuccessErrorDisplay";

const BookingForm = () => {
  const {
    errors,
    formData,
    userProfile,
    isPhoneNumberVerificationProcess,
    isVerifyingPhoneNumber,
    isCodeSent,
    isCreatingBooking,
    bookingSuccess,
    showConfirmation,
    mappedAvailableDates,
    isFetchingAvailableDates,
    errorFetchingAvailableDates,
    canShowCalendar,
    isVerifyingCode,
    hasPhoneError,
    onSubmit,
    handleVerifyCode,
    handleCreateBooking,
    setShowConfirmation,
    updateFormData,
  } = useBookingForm();

  return (
    <>
      <h1 className="text-2xl font-bold mb-12 text-center">
        Book an Appointment
      </h1>
      <Form
        className="flex w-full justify-center items-center space-y-4"
        onSubmit={onSubmit}
        aria-label="Booking appointment form"
      >
        <div className="flex flex-col gap-4 w-full lg:max-w-lg max-w-full px-4 sm:px-0 sm:max-w-md">
          <FormSuccessErrorDisplay
            errors={errors}
            success={bookingSuccess}
            successMessage="Success! You'll be redirected to the bookings list page in a few seconds."
          />

          <div className="flex flex-col gap-2">
            <label htmlFor="phone-input">Phone Number:</label>
            <PhoneInput
              id="phone-input"
              placeholder="Enter phone number"
              value={formData.phoneNumber}
              onChange={(value) => updateFormData("phoneNumber", value)}
              defaultCountry="PL"
              aria-required="true"
              aria-invalid={hasPhoneError}
            />
            {isCodeSent && (
              <p className="text-sm text-gray-500 text-center">
                We&apos;ll send you a verification code to confirm your number.
              </p>
            )}
            {hasPhoneError && (
              <p className="text-red-500" role="alert">
                {
                  errors.find((error) => error.field === "phone-number")
                    ?.message
                }
              </p>
            )}
          </div>

          {isPhoneNumberVerificationProcess ? (
            <PhoneVerificationFormFragment
              errors={errors}
              formData={formData}
              updateVerificationCode={(verificationCode) =>
                updateFormData("verificationCode", verificationCode)
              }
              handleVerifyCode={handleVerifyCode}
              isVerifyingCode={isVerifyingCode}
            />
          ) : (
            <>
              <CalendarContainer
                mappedAvailableDates={mappedAvailableDates}
                updateBookingDate={(date) =>
                  updateFormData("bookingDate", date)
                }
                canShowCalendar={canShowCalendar}
                isFetchingAvailableDates={isFetchingAvailableDates}
                errorFetchingAvailableDates={errorFetchingAvailableDates}
              />

              <TimeSlotListSlider
                selectedDate={formData.bookingDate}
                onSelectTimeSlot={(timeSlot) =>
                  updateFormData("selectedTimeSlot", timeSlot)
                }
                selectedTimeSlot={formData.selectedTimeSlot}
              />

              <Button
                className="w-full"
                color="primary"
                type="submit"
                isLoading={isVerifyingPhoneNumber || isCreatingBooking}
                isDisabled={
                  !formData.selectedTimeSlot ||
                  isVerifyingPhoneNumber ||
                  isCreatingBooking
                }
                aria-label={
                  userProfile?.isPhoneNumberVerified
                    ? "Book Appointment"
                    : "Verify Phone Number"
                }
              >
                {userProfile?.isPhoneNumberVerified
                  ? "Book Appointment"
                  : "Verify Phone Number"}
              </Button>

              {!formData.selectedTimeSlot && (
                <p
                  className="text-sm text-center text-gray-500"
                  aria-live="polite"
                >
                  Please select a time slot to continue
                </p>
              )}
            </>
          )}
        </div>
      </Form>

      {showConfirmation && (
        <BookingConfirmationDialog
          selectedTimeSlot={formData.selectedTimeSlot}
          isCreatingBooking={isCreatingBooking}
          setShowConfirmation={setShowConfirmation}
          handleCreateBooking={handleCreateBooking}
        />
      )}
    </>
  );
};

export default BookingForm;
