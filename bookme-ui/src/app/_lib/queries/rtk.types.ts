import { ApiBookingTimeslotsPostRequest } from "../codegen";

export type CustomBaseQueryType =
  | {
      endpoint: "user.getUserProfile";
      params?: null;
    }
  | {
      endpoint: "booking.createTimeSlot";
      params: { request: ApiBookingTimeslotsPostRequest };
    };