import { TimeSlotDto } from "@/_lib/codegen";
import React from "react";
import { format } from "date-fns";
import { Button } from "@heroui/react";

const TimeSlotSliderItem = ({
  timeSlot,
  onSelectTimeSlot,
  isSelected,
}: {
  timeSlot: TimeSlotDto;
  onSelectTimeSlot: (timeSlot: TimeSlotDto) => void;
  isSelected: boolean;
}) => {
  return (
    <Button
      radius="full"
      className={`min-w-[10rem] text-nowrap px-4 py-2 hover:scale-105 transition-transform ${
        isSelected ? "bg-primary text-white" : ""
      }`}
      color="primary"
      variant="flat"
      onPress={() => onSelectTimeSlot(timeSlot)}
    >
      {format(timeSlot.start as Date, "h:mm a")} -{" "}
      {format(timeSlot.end as Date, "h:mm a")}
    </Button>
  );
};

export default TimeSlotSliderItem;
