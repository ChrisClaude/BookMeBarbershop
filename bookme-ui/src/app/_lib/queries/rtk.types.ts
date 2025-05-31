import {
  ApiBookingBookTimeslotPostRequest,
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
        request: {
          getAvailableTimeSlotsDto: {
            start: string; // ISO string - redux cannot serialize date objects
            end: string; // ISO string
          };
        };
      };
    }
  | {
      endpoint: "booking.getAllTimeSlots";
      params: {
        request: {
          getAvailableTimeSlotsDto: {
            start: string; // ISO string - redux cannot serialize date objects
            end: string; // ISO string
            isAvailable: boolean | null;
          };
          pageIndex?: number;
          pageSize?: number;
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
    };

export type QueryResult<T> = {
  data?: T;
  isFetching: boolean;
  error?: string[];
};
