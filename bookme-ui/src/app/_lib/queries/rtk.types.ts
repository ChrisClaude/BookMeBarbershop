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

export type QueryResult<T> = {
  data?: T;
  isFetching: boolean;
  error?: string[];
};
