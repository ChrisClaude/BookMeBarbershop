import {
  ApiBookingTimeslotsAvailablePostRequest,
  ApiBookingTimeslotsPostRequest,
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

  private static buildHeaders({
    token,
    contentType,
  }: {
    token: string;
    contentType?: string;
  }) {
    const headers = {
      Authorization: `Bearer ${token}`,
      ["content-type"]: contentType || "application/json",
    };

    return headers;
  }

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

  public static async getAvailableTimeSlots({
    request,
  }: {
    request: ApiBookingTimeslotsAvailablePostRequest;
  }): Promise<Result<PagedListDtoOfTimeSlotDto>> {
    try {
      const response = await this.bookMeApi.apiBookingTimeslotsAvailablePostRaw(
        request
      );

      if (response.raw.status !== 200) {
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
    request: ApiBookingTimeslotsAvailablePostRequest;
  }): Promise<Result<PagedListDtoOfTimeSlotDto>> {
    try {
      const response = await this.bookMeApi.apiBookingTimeslotsAllPostRaw(
        request
      );

      if (response.raw.status !== 200) {
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
}
