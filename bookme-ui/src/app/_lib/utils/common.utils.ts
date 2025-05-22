import { logWarning } from "./logging.utils";
import { ModelError as ApiError } from "../codegen";

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
