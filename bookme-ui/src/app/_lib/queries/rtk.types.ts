import {
  ApiBookingBookTimeslotPostRequest,
  ApiBookingGetBookingsPostRequest,
  ApiBookingTimeslotsAllPostRequest,
  ApiBookingTimeslotsAvailablePostRequest,
  ApiBookingTimeslotsPostRequest,
  ApiPhoneVerificationSendCodePostRequest,
  ApiPhoneVerificationVerifyCodePostRequest,
} from "../codegen";

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
        request: Omit<
          ApiBookingTimeslotsAvailablePostRequest,
          "getAvailableTimeSlotsDto"
        > & {
          getAvailableTimeSlotsDto: Omit<
            ApiBookingTimeslotsAvailablePostRequest["getAvailableTimeSlotsDto"],
            "start" | "end"
          > & {
            start: string; // ISO string - redux cannot serialize date objects
            end: string; // ISO string
          };
        };
      };
    }
  | {
      endpoint: "booking.getAllTimeSlots";
      params: {
        request: Omit<ApiBookingTimeslotsAllPostRequest, "getTimeSlotsDto"> & {
          getTimeSlotsDto: Omit<
            ApiBookingTimeslotsAllPostRequest["getTimeSlotsDto"],
            "start" | "end"
          > & {
            start: string; // ISO string - redux cannot serialize date objects
            end: string; // ISO string
          };
        };
      };
    }
  | {
      endpoint: "phoneVerification.verifyPhoneNumber";
      params: {
        request: ApiPhoneVerificationSendCodePostRequest;
      };
    }
  | {
      endpoint: "phoneVerification.verifyCodeNumber";
      params: {
        request: ApiPhoneVerificationVerifyCodePostRequest;
      };
    }
  | {
      endpoint: "booking.createBooking";
      params: {
        request: ApiBookingBookTimeslotPostRequest;
      };
    }
  | {
      endpoint: "booking.getBookings";
      params: {
        request: GetBookingsQueryType;
      };
    };

export type QueryResult<T> = {
  data?: T;
  isFetching: boolean;
  error?: string[];
};

export type GetBookingsQueryType = Omit<
  ApiBookingGetBookingsPostRequest,
  "getBookingsDto"
> & {
  getBookingsDto: Omit<
    ApiBookingGetBookingsPostRequest["getBookingsDto"],
    "fromDateTime"
  > & {
    fromDateTime: string; // ISO string - redux cannot serialize date objects
  };
};
