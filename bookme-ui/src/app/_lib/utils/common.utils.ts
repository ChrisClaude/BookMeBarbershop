import { logWarning } from "./logging.utils";
import { ModelError as ApiError } from "../codegen";
import {
  E164Number,
  isPossiblePhoneNumber,
  isValidPhoneNumber,
  parsePhoneNumberWithError,
} from "libphonenumber-js";

export const isNullOrUndefined = <T>(
  value: T | null | undefined
): value is null | undefined => {
  return value === null || value === undefined;
};

export const isNotNullOrUndefined = <T>(
  value: T | null | undefined
): value is T => {
  return !isNullOrUndefined(value);
};

export const isNullOrWhiteSpace = (
  value: string | null | undefined
): value is null | undefined | "" => {
  return isNullOrUndefined(value) || value?.trim() === "";
};

export const isNotNullOrWhiteSpace = (
  value: string | null | undefined
): value is string => {
  return isNotNullOrUndefined(value) && value?.trim() !== "";
};

export const isANumber = (
  value: string | null | undefined | unknown
): value is number => {
  return isNotNullOrUndefined(value) && !isNaN(Number(value));
};

export const getConfig = (
  key: string,
  value: string | null | undefined
): string => {
  if (isNullOrWhiteSpace(value)) {
    logWarning(`Config value for ${key} is not set`, "getConfig");
    return "";
  }

  return value;
};

export const getErrorsFromApiResult = (errorResult: ApiError[]): string[] => {
  if (Array.isArray(errorResult)) {
    return errorResult.map((error) =>
      typeof error === "object" && "description" in error
        ? (error.description as string)
        : "Unknown error"
    );
  } else if (typeof errorResult === "object" && errorResult !== null) {
    return Object.values(errorResult).map((error) => String(error));
  } else {
    return ["An unknown error occurred."];
  }
};

export const isStatusCodeSuccess = (statusCode: number) => {
  return statusCode >= 200 && statusCode < 300;
};

export const validatePhoneNumber = (
  value: E164Number | undefined
): string[] => {
  const errors: string[] = [];

  if (value) {
    const possible = isPossiblePhoneNumber(value);
    const valid = isValidPhoneNumber(value);

    if (possible && valid) {
      return errors;
    } else if (possible && !valid) {
      errors.push("Invalid phone number format for the selected country.");
      return errors;
    } else {
      errors.push("Not a correct phone number.");
      return errors;
    }
  } else {
    errors.push("Phone number is empty.");
    return errors;
  }
};

export const toE164 = (phone: string): E164Number | undefined => {
  if (isNullOrWhiteSpace(phone)) {
    return undefined;
  }

  try {
    const phoneNumber = parsePhoneNumberWithError(phone);
    if (phoneNumber.isValid()) {
      return phoneNumber.number as E164Number; // returns in E.164 format
    }
  } catch (error) {
    logWarning("Invalid phone number", "toE164", error);
  }
  return undefined;
};
