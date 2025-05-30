"use client";
import { PagedListDtoOfTimeSlotDto } from "@/_lib/codegen";
import { useGetAvailableTimeSlotsQuery } from "@/_lib/queries";
import { QueryResult } from "@/_lib/queries/rtk.types";
import { DateValue, getLocalTimeZone } from "@internationalized/date";
import { format } from "date-fns";
import { useCallback, useEffect, useMemo } from "react";
import TimeSlotSliderItem from "./TimeSlotSliderItem";
import { GoPlusCircle } from "react-icons/go";
import { Button, Pagination, Tooltip } from "@heroui/react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { SLOTS_PER_PAGE_SIZE } from "@/config";

const TimeSlotListSlider = ({
  selectedDate,
  onCreateTimeSlot,
}: {
  selectedDate: DateValue;
  onCreateTimeSlot: () => void;
}) => {
  const router = useRouter();
  const searchParams = useSearchParams();
  const pathName = usePathname();

  const pageIndex = useMemo(() => {
    const page = searchParams.get("page");

    return Number(page) || 1;
  }, [searchParams]);

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
      pageIndex: pageIndex - 1, // pageIndex is 0-based
      pageSize: SLOTS_PER_PAGE_SIZE,
    };
  }, [pageIndex, selectedDate]);

  const handlePageChange = useCallback(
    (page: number) => {
      router.push(`?page=${page}`);
    },
    [router]
  );

  useEffect(() => {
    if (searchParams.size > 0) {
      router.push(pathName);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedDate]);

  const {
    data: timeSlots,
    isFetching,
    error,
  } = useGetAvailableTimeSlotsQuery<QueryResult<PagedListDtoOfTimeSlotDto>>(request);

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
            <TimeSlotSliderItem key={timeSlot.id} timeSlot={timeSlot} />
          ))}
          {shouldShowPagination && (
            <Pagination
              initialPage={timeSlots.pageIndex}
              total={timeSlots.totalPages!}
              onChange={handlePageChange}
              page={pageIndex}
            />
          )}
        </div>
      )}
    </div>
  );
};

export default TimeSlotListSlider;
