import {
  ApiBookingBookTimeslotPostRequest,
  ApiBookingCancelBookingPostRequest,
  ApiBookingGetBookingsAllPostRequest,
  ApiBookingGetBookingsPostRequest,
  ApiBookingTimeslotsAllPostRequest,
  ApiBookingTimeslotsAvailableDatesPostRequest,
  ApiBookingTimeslotsAvailablePostRequest,
  ApiBookingTimeslotsPostRequest,
  ApiPhoneVerificationSendCodePostRequest,
  ApiPhoneVerificationVerifyCodePostRequest,
  ApiUserAllGetRequest,
  ApiUserProfilePutRequest,
} from "../codegen";

export type CustomBaseQueryType =
  | {
      endpoint: "user.getUserProfile";
      params?: null;
    }
  | {
      endpoint: "user.getAllUsers";
      params: { request: ApiUserAllGetRequest };
    }
  | {
      endpoint: "user.updateUserProfile";
      params: { request: ApiUserProfilePutRequest };
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
        request: GetAllTimeSlotsQueryType;
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
    }
  | {
      endpoint: "booking.getAllBookings";
      params: {
        request: GetAllBookingQueryType;
      };
    }
  | {
      endpoint: "booking.cancelBooking";
      params: {
        request: ApiBookingCancelBookingPostRequest;
      };
    }
  | {
      endpoint: "booking.getAvailableDates";
      params: {
        request: GetAllAvailableDatesQueryType;
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

export type GetAllBookingQueryType = Omit<
  ApiBookingGetBookingsAllPostRequest,
  "getBookingsDto"
> & {
  getBookingsDto: Omit<
    ApiBookingGetBookingsAllPostRequest["getBookingsDto"],
    "fromDateTime"
  > & {
    fromDateTime: string; // ISO string - redux cannot serialize date objects
  };
};

export type GetAllTimeSlotsQueryType = Omit<
  ApiBookingTimeslotsAllPostRequest,
  "getTimeSlotsDto"
> & {
  getTimeSlotsDto: Omit<
    ApiBookingTimeslotsAllPostRequest["getTimeSlotsDto"],
    "start" | "end"
  > & {
    start: string; // ISO string - redux cannot serialize date objects
    end: string; // ISO string
  };
};

export type GetAllAvailableDatesQueryType = Omit<
  ApiBookingTimeslotsAvailableDatesPostRequest,
  "getAvailableDatesDto"
> & {
  getAvailableDatesDto: Omit<
    ApiBookingTimeslotsAvailableDatesPostRequest["getAvailableDatesDto"],
    "start" | "end"
  > & {
    start: string; // ISO string - redux cannot serialize date objects
    end: string; // ISO string
  };
};
