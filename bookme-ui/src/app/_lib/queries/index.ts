import { createApi } from "@reduxjs/toolkit/query/react";
import { customBaseQuery } from "./customBaseQuery";

export const api = createApi({
  reducerPath: "api",
  baseQuery: customBaseQuery(),
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
    }),
    getAvailableTimeSlots: builder.query({
      query: (request) => ({
        endpoint: "booking.getAvailableTimeSlots",
        params: { request },
      }),
    }),
    //#endregion
  }),
});

export const { useGetUserProfileQuery, useGetAvailableTimeSlotsQuery, useCreateTimeSlotMutation } = api;
