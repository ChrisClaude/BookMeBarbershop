"use client";
import { TimeSlotDto } from "@/_lib/codegen";
import { useGetAvailableTimeSlotsQuery } from "@/_lib/queries";
import { QueryResult } from "@/_lib/queries/rtk.types";
import { parseDate } from "@/_lib/utils/dateUtils";
import { DateValue, getLocalTimeZone } from "@internationalized/date";
import { format } from "date-fns";
import { useMemo } from "react";

const TimeSlotList = ({ selectedDate }: { selectedDate: DateValue }) => {
  const request = useMemo(() => {
    const startDate = selectedDate.toDate(getLocalTimeZone());
    const endDate = selectedDate.toDate(getLocalTimeZone());
    startDate.setHours(0, 0, 0);
    endDate.setHours(23, 59, 59);

    return {
      getAvailableTimeSlotsDto: {
        start: startDate.toISOString(),
        end: endDate.toISOString(),
      },
    };
  }, [selectedDate]);

  const {
    data: timeSlots,
    isFetching,
    error,
  } = useGetAvailableTimeSlotsQuery<QueryResult<TimeSlotDto[]>>(request);

  if (isFetching) {
    return <div>Loading...</div>;
  }

  if (error || !timeSlots) {
    return <div>Error: {error}</div>;
  }

  const formattedDate = format(selectedDate.toDate(getLocalTimeZone()), "EEEE, MMMM d, yyyy");

  return (
    <div className="space-y-4">
      <h2 className="text-xl font-semibold mb-2">Available Time Slots for {formattedDate}</h2>
      {timeSlots.length === 0 ? (
        <div className="p-4 border rounded-lg bg-gray-50 text-center">
          <p className="text-gray-500">No time slots available for this date</p>
        </div>
      ) : (
        <div className="grid gap-3">
          {timeSlots.map((timeSlot) => (
            <div
              key={timeSlot.id}
              className={`p-4 border rounded-lg shadow-sm transition-all hover:shadow-md ${
                timeSlot.isAvailable ? 'bg-green-50 border-green-200' : 'bg-red-50 border-red-200'
              }`}
            >
              <div className="flex justify-between items-center">
                <div>
                  <p className="font-medium">{format(parseDate(timeSlot.start), "h:mm a")} - {format(parseDate(timeSlot.end), "h:mm a")}</p>
                  <p className="text-sm text-gray-500">{format(parseDate(timeSlot.start), "EEEE, MMMM d")}</p>
                </div>
                <div className="flex items-center">
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                    timeSlot.isAvailable ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                  }`}>
                    {timeSlot.isAvailable ? 'Available' : 'Booked'}
                  </span>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default TimeSlotList;
