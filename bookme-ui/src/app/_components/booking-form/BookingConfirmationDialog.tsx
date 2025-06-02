import { TimeSlotDto } from "@/_lib/codegen";
import { Button } from "@heroui/react";
import { format } from "date-fns";
import React from "react";
import useBookingForm from "./useBookingForm";

const BookingConfirmationDialog = ({
  selectedTimeSlot,
  isCreatingBooking,
  setShowConfirmation,
  setBookingSuccess,
}: {
  selectedTimeSlot: TimeSlotDto | undefined;
  isCreatingBooking: boolean;
  setShowConfirmation: (show: boolean) => void;
  setBookingSuccess: (success: boolean) => void;
}) => {
  const { createBooking } = useBookingForm();
  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white p-6 rounded-lg max-w-md w-full">
        <h3 className="text-xl font-bold mb-4">Confirm your booking</h3>
        <p>Date: {format(selectedTimeSlot?.start as Date, "EEEE, MMMM d")}</p>
        <p>
          Time: {format(selectedTimeSlot?.start as Date, "h:mm a")} -{" "}
          {format(selectedTimeSlot?.end as Date, "h:mm a")}
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
                  timeSlotId: selectedTimeSlot?.id,
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
  );
};

export default BookingConfirmationDialog;
