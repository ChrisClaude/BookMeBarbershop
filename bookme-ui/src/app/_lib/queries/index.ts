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
    //#endregion
  }),
});

export const {
  useGetUserProfileQuery,
  useGetAvailableTimeSlotsQuery,
  useGetAllTimeSlotsQuery,
  useCreateTimeSlotMutation,
} = api;
