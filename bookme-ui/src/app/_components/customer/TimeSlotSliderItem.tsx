import { TimeSlotDto } from "@/_lib/codegen";
import React from "react";
import { format } from "date-fns";

const TimeSlotSliderItem = ({ timeSlot }: { timeSlot: TimeSlotDto }) => {
  return (
    <div
      key={timeSlot.id}
      className={`p-4 border rounded-lg shadow-sm transition-all hover:shadow-md ${
        timeSlot.isAvailable
          ? "bg-green-50 border-green-200 hover:bg-green-100"
          : "bg-red-50 border-red-200 hover:bg-red-100"
      }`}
    >
      <div className="flex justify-between items-center">
        <div>
          <p className="font-medium">
            {format(timeSlot.start as Date, "h:mm a")} -{" "}
            {format(timeSlot.end as Date, "h:mm a")}
          </p>
          <p className="text-sm text-gray-500">
            {format(timeSlot.start as Date, "EEEE, MMMM d")}
          </p>
        </div>
        <div className="flex items-center">
          <span
            className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
              timeSlot.isAvailable
                ? "bg-green-100 text-green-800"
                : "bg-red-100 text-red-800"
            }`}
          >
            {timeSlot.isAvailable ? "Available" : "Booked"}
          </span>
        </div>
      </div>
    </div>
  );
};

export default TimeSlotSliderItem;
