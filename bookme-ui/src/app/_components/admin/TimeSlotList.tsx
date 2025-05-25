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

  // console.log("local timezone", getLocalTimeZone());

  return (
    <div>
      {timeSlots.map((timeSlot) => (
        <div key={timeSlot.id}>
          <p>{format(parseDate(timeSlot.start), "PPP p")}</p>
          <p>{format(parseDate(timeSlot.end), "PPP p")}</p>
          <p>{timeSlot.isAvailable}</p>
        </div>
      ))}
    </div>
  );
};

export default TimeSlotList;
