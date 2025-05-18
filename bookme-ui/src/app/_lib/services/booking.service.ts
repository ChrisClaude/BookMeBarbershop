import { ApiBookingTimeslotsPostRequest, TimeSlotDto } from "../codegen";
import { Result } from "../types/common.types";
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
    token,
  }: {
    request: ApiBookingTimeslotsPostRequest;
    token: string;
  }): Promise<Result<TimeSlotDto>> {
    try {
      const headers = this.buildHeaders({ token });
      const response = await this.bookMeApi.apiBookingTimeslotsPostRaw(request, {
        headers,
      });

      if (response.raw.status !== 200) {
        const error = await response.raw.json();
        return {
          success: false,
          errors: [error?.toString() || "Error fetching user"],
        };
      }
      const body = await response.raw.json();
      return {
        success: true,
        data: body as TimeSlotDto,
      };
    } catch (error) {
      logError(
        error?.toString() || "Error fetching user",
        "GetUserError",
        error
      );
      return {
        success: false,
        errors: [error?.toString() || "Error fetching user"],
      };
    }
  }
}
