import { BookingService } from "../services/booking.service";
import { PhoneVerificationService } from "../services/phoneVerification.service";
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

      case "booking.createBooking":
        return transformRTKResult(
          await BookingService.createBooking({
            ...params,
            request: params.request,
          })
        );

      case "booking.getAllTimeSlots":
        return transformRTKResult(
          await BookingService.getAllTimeSlots({
            ...params,
            request: {
              ...params.request,
              getTimeSlotsDto: {
                start: new Date(params.request.getAvailableTimeSlotsDto.start),
                end: new Date(params.request.getAvailableTimeSlotsDto.end),
              },
              pageIndex: params.request.pageIndex,
              pageSize: params.request.pageSize,
            },
          })
        );

      case "phoneVerification.verifyPhoneNumber":
        return transformRTKResult(
          await PhoneVerificationService.verifyPhoneNumber({
            ...params,
            request: params.request,
          })
        );

      case "phoneVerification.verifyCodeNumber":
        return transformRTKResult(
          await PhoneVerificationService.verifyCodeNumber({
            ...params,
            request: params.request,
          })
        );

      default:
        throw new Error(`Unknown endpoint: ${endpoint}`);
    }
  };
