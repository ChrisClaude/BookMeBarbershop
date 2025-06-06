import { createApi } from "@reduxjs/toolkit/query/react";
import { customBaseQuery } from "./customBaseQuery";

export const api = createApi({
  reducerPath: "api",
  baseQuery: customBaseQuery(),
  tagTypes: ["TimeSlots", "User", "Booking"],
  endpoints: (builder) => ({
    //#region User
    getUserProfile: builder.query({
      query: () => ({
        endpoint: "user.getUserProfile",
        params: null,
      }),
      providesTags: ["User"],
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
    getAvailableDates: builder.query({
      query: (request) => ({
        endpoint: "booking.getAvailableDates",
        params: { request },
      }),
      providesTags: ["TimeSlots"],
    }),

    createBooking: builder.mutation({
      query: (request) => ({
        endpoint: "booking.createBooking",
        params: { request },
      }),
      invalidatesTags: ["Booking"],
    }),

    cancelBooking: builder.mutation({
      query: (request) => ({
        endpoint: "booking.cancelBooking",
        params: { request },
      }),
      invalidatesTags: ["Booking"],
    }),

    getBookings: builder.query({
      query: (request) => ({
        endpoint: "booking.getBookings",
        params: { request },
      }),
      providesTags: ["Booking"],
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
      invalidatesTags: ["User"], // invalidate user profile to refetch it and get the updated phone number verification status
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
  useCancelBookingMutation,
  useGetBookingsQuery,
  useGetAvailableDatesQuery,
} = api;
