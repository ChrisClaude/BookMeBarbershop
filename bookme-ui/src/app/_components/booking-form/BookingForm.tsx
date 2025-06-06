"use client";
import React from "react";
import { Button, Calendar, DatePicker, DateValue, Form } from "@heroui/react";
import { getLocalTimeZone, today } from "@internationalized/date";
import PhoneInput from "react-phone-number-input";
import TimeSlotListSlider from "../customer/TimeSlotListSlider";
import useBookingForm from "./useBookingForm";
import BookingConfirmationDialog from "./BookingConfirmationDialog";
import PhoneVerificationFormFragment from "./PhoneVerificationFragment";

const BookingForm = () => {
  const {
    errors,
    formData,
    userProfile,
    setFormData,
    isPhoneNumberVerificationProcess,
    isVerifyingPhoneNumber,
    isCodeSent,
    isVerifyingCode,
    isCreatingBooking,
    bookingSuccess,
    showConfirmation,
    onSubmit,
    handleVerifyCode,
    handleCreateBooking,
    setShowConfirmation,
  } = useBookingForm();

  return (
    <>
      <h1 className="text-2xl font-bold mb-12 text-center">
        Book an Appointment
      </h1>
      <Form
        className="w-full justify-center items-center space-y-4"
        onSubmit={onSubmit}
      >
        <div className="flex flex-col gap-4 w-full lg:max-w-lg max-w-full px-4 sm:px-0 sm:max-w-md">
          {errors.length > 0 && (
            <div className="p-4 border rounded-lg bg-red-50 text-center">
              {errors.map((error, index) => (
                <p className="text-red-500" key={index}>
                  {error.field} - {error.message}
                </p>
              ))}
            </div>
          )}
          {bookingSuccess && (
            <div className="p-4 border rounded-lg bg-green-50 text-center mb-4">
              <p className="text-green-600">
                Success! You&apos;ll be redirected to the bookings list page in
                a few seconds.
              </p>
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
            {isCodeSent && (
              <p className="text-sm text-gray-500 text-center">
                We&apos;ll send you a verification code to confirm your number.
              </p>
            )}
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
            <PhoneVerificationFormFragment
              errors={errors}
              formData={formData}
              setFormData={setFormData}
              handleVerifyCode={handleVerifyCode}
              isVerifyingCode={isVerifyingCode}
            />
          )}

          {!isPhoneNumberVerificationProcess && (
            <>
              <Calendar
                aria-label="appointment-date"
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
                visibleMonths={2}
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
                isLoading={isVerifyingPhoneNumber || isCreatingBooking}
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
