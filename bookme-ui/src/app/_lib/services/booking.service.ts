import {
  ApiBookingBookTimeslotPostRequest,
  ApiBookingCancelBookingPostRequest,
  ApiBookingGetBookingsPostRequest,
  ApiBookingTimeslotsAllPostRequest,
  ApiBookingTimeslotsAvailablePostRequest,
  ApiBookingTimeslotsPostRequest,
  BookingDto,
  PagedListDtoOfBookingDto,
  PagedListDtoOfTimeSlotDto,
  TimeSlotDto,
} from "../codegen";
import { Result } from "../types/common.types";
import {
  getErrorsFromApiResult,
  isStatusCodeSuccess,
} from "../utils/common.utils";
import { logError } from "../utils/logging.utils";
import { BookingApiWithConfig } from "./api.service";

export class BookingService {
  protected static bookMeApi = new BookingApiWithConfig();

  public static async createTimeSlot({
    request,
  }: {
    request: ApiBookingTimeslotsPostRequest;
  }): Promise<Result<TimeSlotDto>> {
    try {
      const response = await this.bookMeApi.apiBookingTimeslotsPostRaw(request);

      if (!isStatusCodeSuccess(response.raw.status)) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: getErrorsFromApiResult(error),
        };
      }
      const body = await response.raw.json();
      return {
        success: true,
        data: body as TimeSlotDto,
      };
    } catch (error) {
      logError(
        error?.toString() || "Error creating time slot",
        "CreateTimeSlotError",
        error
      );
      return {
        success: false,
        errors: [error?.toString() || "Error creating time slot"],
      };
    }
  }

  public static async createBooking({
    request,
  }: {
    request: ApiBookingBookTimeslotPostRequest;
  }): Promise<Result<BookingDto>> {
    try {
      const response = await this.bookMeApi.apiBookingBookTimeslotPostRaw(
        request
      );

      if (!isStatusCodeSuccess(response.raw.status)) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: getErrorsFromApiResult(error),
        };
      }
      const body = await response.raw.json();
      return {
        success: true,
        data: body as BookingDto,
      };
    } catch (error) {
      logError(
        error?.toString() || "Error booking time slot",
        "CreateBookingError",
        error
      );
      return {
        success: false,
        errors: [error?.toString() || "Error booking time slot"],
      };
    }
  }

  public static async getAvailableTimeSlots({
    request,
  }: {
    request: ApiBookingTimeslotsAvailablePostRequest;
  }): Promise<Result<PagedListDtoOfTimeSlotDto>> {
    try {
      const response = await this.bookMeApi.apiBookingTimeslotsAvailablePostRaw(
        request
      );

      if (!isStatusCodeSuccess(response.raw.status)) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: getErrorsFromApiResult(error),
        };
      }
      const body = await response.raw.json();
      return {
        success: true,
        data: body as PagedListDtoOfTimeSlotDto,
      };
    } catch (error) {
      logError(
        error?.toString() || "Error fetching time slots",
        "GetAvailableTimeSlotsError",
        error
      );
      return {
        success: false,
        errors: [error?.toString() || "Error fetching time slots"],
      };
    }
  }

  public static async getAllTimeSlots({
    request,
  }: {
    request: ApiBookingTimeslotsAllPostRequest;
  }): Promise<Result<PagedListDtoOfTimeSlotDto>> {
    try {
      const response = await this.bookMeApi.apiBookingTimeslotsAllPostRaw(
        request
      );

      if (!isStatusCodeSuccess(response.raw.status)) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: getErrorsFromApiResult(error),
        };
      }
      const body = await response.raw.json();
      return {
        success: true,
        data: body as PagedListDtoOfTimeSlotDto,
      };
    } catch (error) {
      logError(
        error?.toString() || "Error fetching time slots",
        "GetAllTimeSlotsError",
        error
      );
      return {
        success: false,
        errors: [error?.toString() || "Error fetching time slots"],
      };
    }
  }

  public static async getBookings({
    request,
  }: {
    request: ApiBookingGetBookingsPostRequest;
  }): Promise<Result<PagedListDtoOfBookingDto>> {
    try {
      const response = await this.bookMeApi.apiBookingGetBookingsPostRaw(
        request
      );

      if (!isStatusCodeSuccess(response.raw.status)) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: getErrorsFromApiResult(error),
        };
      }
      const body = await response.raw.json();
      return {
        success: true,
        data: body as PagedListDtoOfBookingDto,
      };
    } catch (error) {
      logError(
        error?.toString() || "Error fetching bookings",
        "GetBookingsError",
        error
      );
      return {
        success: false,
        errors: [error?.toString() || "Error fetching bookings"],
      };
    }
  }

  public static async cancelBooking({
    request,
  }: {
    request: ApiBookingCancelBookingPostRequest;
  }): Promise<Result<boolean>> {
    try {
      const response = await this.bookMeApi.apiBookingCancelBookingPostRaw(request);

      if (!isStatusCodeSuccess(response.raw.status)) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: getErrorsFromApiResult(error),
        };
      }
      return {
        success: true,
        data: true,
      };
    } catch (error) {
      logError(
        error?.toString() || "Error cancelling booking",
        "CancelBookingError",
        error
      );
      return {
        success: false,
        errors: [error?.toString() || "Error cancelling booking"],
      };
    }
  }
}
