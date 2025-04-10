import { Session } from "next-auth";

export type Result<T = any> = {
  success: boolean;
  message?: string | any;
  errors?: string[];
  data?: T;
};

export type ApiError = {
  description: string;
  code: string;
};

export type UserSession = Session & {
  accessToken: string;
  externalProvider?: string;
};
