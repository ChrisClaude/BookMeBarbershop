import { ApiBookingBookTimeslotPostRequest, TimeSlotDto } from "@/_lib/codegen";
import { Button } from "@heroui/react";
import { format } from "date-fns";
import React from "react";

const BookingConfirmationDialog = ({
  selectedTimeSlot,
  isCreatingBooking,
  setShowConfirmation,
  handleCreateBooking,
}: {
  selectedTimeSlot: TimeSlotDto | undefined;
  isCreatingBooking: boolean;
  setShowConfirmation: (show: boolean) => void;
  handleCreateBooking: (request: ApiBookingBookTimeslotPostRequest) => void;
}) => {
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
              handleCreateBooking({
                bookTimeSlotsDto: {
                  timeSlotId: selectedTimeSlot?.id,
                },
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
