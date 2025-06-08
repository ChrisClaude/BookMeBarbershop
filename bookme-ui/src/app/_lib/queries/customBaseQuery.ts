import { BookingService } from "../services/booking.service";
import { PhoneVerificationService } from "../services/phoneVerification.service";
import { UserService } from "../services/user.service";
import { CustomBaseQueryType } from "./rtk.types";
import { transformToRTKResult } from "./rtkHelpers";

export const customBaseQuery =
  () =>
  async ({ endpoint, params }: CustomBaseQueryType) => {
    switch (endpoint) {
      //#region User
      case "user.getUserProfile":
        return transformToRTKResult(await UserService.getUserProfile());

      case "user.getAllUsers":
        return transformToRTKResult(
          await UserService.getAllUsers({
            ...params,
            request: params.request,
          })
        );

      case "user.updateUserProfile":
        return transformToRTKResult(
          await UserService.updateUserProfile({
            ...params,
            request: params.request,
          })
        );
      //#endregion

      //#region Booking and TimeSlots
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

      case "booking.createBooking":
        return transformToRTKResult(
          await BookingService.createBooking({
            ...params,
            request: params.request,
          })
        );

      case "booking.getBookings":
        console.log("booking.getBookings", params.request);

        return transformToRTKResult(
          await BookingService.getUserBookings({
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

      case "booking.getAvailableDates":
        return transformToRTKResult(
          await BookingService.getAvailableDates({
            ...params,
            request: {
              ...params.request,
              getAvailableDatesDto: {
                start: new Date(params.request.getAvailableDatesDto.start),
                end: new Date(params.request.getAvailableDatesDto.end),
              },
            },
          })
        );
      //#endregion

      //#region PhoneVerification
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
      //#endregion

      default:
        throw new Error(`Unknown endpoint: ${endpoint}`);
    }
  };
