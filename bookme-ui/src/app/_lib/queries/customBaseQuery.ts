import { BookingService } from "../services/booking.service";
import { UserService } from "../services/user.service";
import { CustomBaseQueryType } from "./rtk.types";
import { transformRTKResult } from "./rtkHelpers";

export const customBaseQuery =
  () =>
  async ({ endpoint, params }: CustomBaseQueryType) => {
    switch (endpoint) {
      case "user.getUserProfile":
        return transformRTKResult(await UserService.getUserProfile());
      case "booking.createTimeSlot":
        return transformRTKResult(await BookingService.createTimeSlot(params));
      case "booking.getAvailableTimeSlots":

        return transformRTKResult(
          await BookingService.getAvailableTimeSlots({
            ...params,
            request: {
              ...params.request,
              getAvailableTimeSlotsDto: {
                start: new Date(params.request.getAvailableTimeSlotsDto.start),
                end: new Date(params.request.getAvailableTimeSlotsDto.end),
              },
            },
          })
        );
      default:
        throw new Error(`Unknown endpoint: ${endpoint}`);
    }
  };
