"use client";
import { PagedListDtoOfTimeSlotDto } from "@/_lib/codegen";
import { useGetAllTimeSlotsQuery } from "@/_lib/queries";
import { QueryResult } from "@/_lib/queries/rtk.types";
import { DateValue, getLocalTimeZone } from "@internationalized/date";
import { format } from "date-fns";
import { useMemo } from "react";
import TimeSlotItem from "./TimeSlotItem";
import { GoPlusCircle } from "react-icons/go";
import { Button, Pagination, Tooltip } from "@heroui/react";

const TimeSlotList = ({
  selectedDate,
  onCreateTimeSlot,
}: {
  selectedDate: DateValue;
  onCreateTimeSlot: () => void;
}) => {
  const request = useMemo(() => {
    const startDate = selectedDate.toDate(getLocalTimeZone());
    const endDate = selectedDate.toDate(getLocalTimeZone());
    startDate.setHours(0, 0, 0);
    endDate.setHours(23, 59, 59);

    return {
      getAvailableTimeSlotsDto: {
        start: startDate.toISOString(),
        end: endDate.toISOString(),
        // TODO: Add ability to filter by availability
        isAvailable: null,
      },
    };
  }, [selectedDate]);

  const {
    data: timeSlots,
    isFetching,
    error,
  } = useGetAllTimeSlotsQuery<QueryResult<PagedListDtoOfTimeSlotDto>>(request);

  const shouldShowPagination = useMemo(
    () => timeSlots && timeSlots.totalPages! > 1,
    [timeSlots]
  );

  if (isFetching) {
    return (
      <div className="p-4 border rounded-lg bg-gray-50 text-center">
        <p className="text-gray-500">Loading time slots...</p>
      </div>
    );
  }

  if (error || !timeSlots || !timeSlots.items) {
    return (
      <div className="p-4 border rounded-lg bg-red-50 text-center">
        <p className="text-red-500">Error loading time slots: {error}</p>
      </div>
    );
  }

  const formattedDate = format(
    selectedDate.toDate(getLocalTimeZone()),
    "EEEE, MMMM d, yyyy"
  );

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center mb-2">
        <h2 className="text-xl font-semibold">
          Available Time Slots for {formattedDate}
        </h2>
        <Tooltip content="Create Time Slot">
          <Button
            isIconOnly
            aria-label="Like"
            color="default"
            onPress={onCreateTimeSlot}
          >
            <GoPlusCircle size={25} />
          </Button>
        </Tooltip>
      </div>
      {timeSlots.items.length === 0 ? (
        <div className="p-4 border rounded-lg bg-gray-50 text-center">
          <p className="text-gray-500">No time slots available for this date</p>
        </div>
      ) : (
        <div className="grid gap-3">
          {timeSlots.items.map((timeSlot) => (
            <TimeSlotItem key={timeSlot.id} timeSlot={timeSlot} />
          ))}
          {shouldShowPagination && (
            <Pagination
              initialPage={timeSlots.pageIndex}
              total={timeSlots.totalPages!}
            />
          )}
        </div>
      )}
    </div>
  );
};

export default TimeSlotList;
