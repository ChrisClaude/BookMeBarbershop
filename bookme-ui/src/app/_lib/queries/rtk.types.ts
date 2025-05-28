import { ApiBookingTimeslotsPostRequest } from "../codegen";

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
      params: {
        request: {
          getAvailableTimeSlotsDto: {
            start: string; // ISO string - redux cannot serialize date objects
            end: string; // ISO string
          };
        };
      };
    } | {
      endpoint: "booking.getAllTimeSlots";
      params: {
        request: {
          getAvailableTimeSlotsDto: {
            start: string; // ISO string - redux cannot serialize date objects
            end: string; // ISO string
            isAvailable: boolean;
          };
        };
      };
    };

export type QueryResult<T> = {
  data?: T;
  isFetching: boolean;
  error?: string[];
};
