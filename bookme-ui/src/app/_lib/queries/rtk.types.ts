import { ApiBookingTimeslotsPostRequest, TimeslotsGetRequest } from "../codegen";

export type CustomBaseQueryType =
  | {
      endpoint: "user.getUserProfile";
      params?: null;
    }
  | {
      endpoint: "booking.createTimeSlot";
      params: { request: ApiBookingTimeslotsPostRequest };
    }
  | {
      endpoint: "booking.getAvailableTimeSlots";
      params: { request: TimeslotsGetRequest };
    };

export type QueryResult<T> = {
  data?: T;
  isFetching: boolean;
  error?: string[];
};
