"use client";
import { Button, DatePicker, DateValue, Form } from "@heroui/react";
import { getLocalTimeZone, today } from "@internationalized/date";
import React, { Fragment } from "react";
import PhoneInput from "react-phone-number-input";
import TimeSlotListSlider from "../customer/TimeSlotListSlider";
import { format } from "date-fns";
import useBookingForm from "./useBookingForm";

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
    createBooking,
    setBookingSuccess,
    setShowConfirmation,
  } = useBookingForm();

  return (
    <>
      <h1 className="text-2xl font-bold mb-12 text-center">
        Book your appointment
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
                Your appointment has been booked successfully!
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
            </Fragment>
          )}
        </div>
      </Form>
      {showConfirmation && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg max-w-md w-full">
            <h3 className="text-xl font-bold mb-4">Confirm your booking</h3>
            <p>Date: {formData.bookingDate.toString()}</p>
            <p>
              Time: {format(formData.selectedTimeSlot?.start as Date, "h:mm a")}{" "}
              - {format(formData.selectedTimeSlot?.end as Date, "h:mm a")}
            </p>
            <div className="flex gap-2 mt-4">
              <Button
                color="danger"
                variant="light"
                onPress={() => setShowConfirmation(false)}
              >
                Cancel
              </Button>
              <Button
                color="primary"
                isLoading={isCreatingBooking}
                onPress={() => {
                  createBooking({
                    bookTimeSlotsDto: {
                      timeSlotId: formData.selectedTimeSlot?.id,
                    },
                  })
                    .unwrap()
                    .then(() => {
                      setBookingSuccess(true);
                      setShowConfirmation(false);
                    });
                }}
              >
                Confirm Booking
              </Button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default BookingForm;
