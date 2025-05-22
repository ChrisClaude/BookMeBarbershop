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
      default:
        throw new Error(`Unknown endpoint: ${endpoint}`);
    }
  };
