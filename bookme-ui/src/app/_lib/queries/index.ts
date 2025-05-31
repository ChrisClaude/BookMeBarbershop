import { createApi } from "@reduxjs/toolkit/query/react";
import { customBaseQuery } from "./customBaseQuery";

export const api = createApi({
  reducerPath: "api",
  baseQuery: customBaseQuery(),
  tagTypes: ["TimeSlots"],
  endpoints: (builder) => ({
    //#region User
    getUserProfile: builder.query({
      query: () => ({
        endpoint: "user.getUserProfile",
        params: null,
      }),
    }),
    //#endregion

    //#region Booking
    createTimeSlot: builder.mutation({
      query: (request) => ({
        endpoint: "booking.createTimeSlot",
        params: { request },
      }),
      invalidatesTags: ["TimeSlots"],
    }),
    getAvailableTimeSlots: builder.query({
      query: (request) => ({
        endpoint: "booking.getAvailableTimeSlots",
        params: { request },
      }),
      providesTags: ["TimeSlots"],
    }),
    getAllTimeSlots: builder.query({
      query: (request) => ({
        endpoint: "booking.getAllTimeSlots",
        params: { request },
      }),
      providesTags: ["TimeSlots"],
    }),

    createBooking: builder.mutation({
      query: (request) => ({
        endpoint: "booking.createBooking",
        params: { request },
      }),
    }),
    //#endregion

    //#region PhoneVerification
    verifyPhoneNumber: builder.mutation({
      query: (request) => ({
        endpoint: "phoneVerification.verifyPhoneNumber",
        params: { request },
      }),
    }),
    verifyCodeNumber: builder.mutation({
      query: (request) => ({
        endpoint: "phoneVerification.verifyCodeNumber",
        params: { request },
      }),
    }),
    //#endregion
  }),
});

export const {
  useGetUserProfileQuery,
  useGetAvailableTimeSlotsQuery,
  useGetAllTimeSlotsQuery,
  useCreateTimeSlotMutation,
  useVerifyPhoneNumberMutation,
  useVerifyCodeNumberMutation,
  useCreateBookingMutation,
} = api;
