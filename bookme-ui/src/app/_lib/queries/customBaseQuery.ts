import { BookingService } from "../services/booking.service";
import { PhoneVerificationService } from "../services/phoneVerification.service";
import { UserService } from "../services/user.service";
import { CustomBaseQueryType } from "./rtk.types";
import { transformToRTKResult } from "./rtkHelpers";

export const customBaseQuery =
  () =>
  async ({ endpoint, params }: CustomBaseQueryType) => {
    switch (endpoint) {
      case "user.getUserProfile":
        return transformToRTKResult(await UserService.getUserProfile());
      case "booking.createTimeSlot":
        return transformToRTKResult(
          await BookingService.createTimeSlot(params)
        );
      case "booking.getAvailableTimeSlots":
        return transformToRTKResult(
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
        return transformToRTKResult(
          await BookingService.createBooking({
            ...params,
            request: params.request,
          })
        );

      case "booking.getAllTimeSlots":
        return transformToRTKResult(
          await BookingService.getAllTimeSlots({
            ...params,
            request: {
              ...params.request,
              getTimeSlotsDto: {
                start: new Date(params.request.getTimeSlotsDto.start),
                end: new Date(params.request.getTimeSlotsDto.end),
              },
            },
          })
        );

      case "booking.getBookings":
        console.log("booking.getBookings", params.request);

        return transformToRTKResult(
          await BookingService.getBookings({
            ...params,
            request: {
              ...params.request,
              getBookingsDto: {
                fromDateTime: new Date(
                  params.request.getBookingsDto.fromDateTime
                ),
                bookingStatus: params.request.getBookingsDto.bookingStatus,
              },
            },
          })
        );

      case "booking.cancelBooking":
        return transformToRTKResult(
          await BookingService.cancelBooking({
            ...params,
            request: params.request,
          })
        );

      case "phoneVerification.verifyPhoneNumber":
        return transformToRTKResult(
          await PhoneVerificationService.verifyPhoneNumber({
            ...params,
            request: params.request,
          })
        );

      case "phoneVerification.verifyCodeNumber":
        return transformToRTKResult(
          await PhoneVerificationService.verifyCodeNumber({
            ...params,
            request: params.request,
          })
        );

      default:
        throw new Error(`Unknown endpoint: ${endpoint}`);
    }
  };
