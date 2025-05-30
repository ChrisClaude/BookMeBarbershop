"use client";
import { PagedListDtoOfTimeSlotDto } from "@/_lib/codegen";
import { useGetAvailableTimeSlotsQuery } from "@/_lib/queries";
import { QueryResult } from "@/_lib/queries/rtk.types";
import { DateValue, getLocalTimeZone } from "@internationalized/date";
import { useCallback, useEffect, useMemo } from "react";
import TimeSlotSliderItem from "./TimeSlotSliderItem";
import { Button } from "@heroui/react";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { SLOTS_PER_PAGE_SIZE } from "@/config";
import { MdNavigateNext } from "react-icons/md";

const TimeSlotListSlider = ({ selectedDate }: { selectedDate: DateValue }) => {
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
  } = useGetAvailableTimeSlotsQuery<QueryResult<PagedListDtoOfTimeSlotDto>>(
    request
  );

  const shouldShowNextSlotsButton = useMemo(
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

  return (
    <div className="space-y-4">
      {timeSlots.items.length === 0 ? (
        <div className="p-4 border rounded-lg bg-gray-50 text-center">
          <p className="text-gray-500">No time slots available for this date</p>
        </div>
      ) : (
        <div className="flex justify-between items-center gap-3">
          <div className="flex gap-2 overflow-x-auto p-2">
            {timeSlots.items.map((timeSlot) => (
              <TimeSlotSliderItem key={timeSlot.id} timeSlot={timeSlot} />
            ))}
          </div>
          {shouldShowNextSlotsButton && (
            <Button
              isIconOnly
              aria-label="next"
              onPress={() => handlePageChange(pageIndex + 1)}
            >
              <MdNavigateNext />
            </Button>
          )}
        </div>
      )}
    </div>
  );
};

export default TimeSlotListSlider;
