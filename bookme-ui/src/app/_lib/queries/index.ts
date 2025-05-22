import { createApi } from "@reduxjs/toolkit/query/react";
import { customBaseQuery } from "./customBaseQuery";

export const userApi = createApi({
  reducerPath: 'api',
  baseQuery: customBaseQuery(),
  endpoints: (builder) => ({
    getUserProfile: builder.query({
     query: () => ({
        endpoint: 'user.getUserProfile',
        params: null,
      }),
    }),
  }),
})

export const { useGetUserProfileQuery } = userApi