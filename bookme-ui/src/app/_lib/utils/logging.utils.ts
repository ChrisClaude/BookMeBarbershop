import { appInsights } from "@/appInsights";
import { isNotNullOrWhiteSpace, isNullOrUndefined } from "./common.utils";
import { APP_INSIGHTS_CONNECTION_STRING } from "@/config";

const hasExternalLoggingService = isNotNullOrWhiteSpace(
  APP_INSIGHTS_CONNECTION_STRING
);
/**
 * This method provides a wrapper around the error logging mechanism, so that later when an external logging provider is introduced we can do the switch in one place
 * @param message The error message
 * @param name The error name
 * @param cause The error cause
 * @param stack The error stack
 * @returns
 */
export const logError = (
  message: string,
  name: string,
  cause: string | undefined | unknown = undefined,
  stack: string | undefined = undefined
): void => {
  if (!hasExternalLoggingService || isNullOrUndefined(appInsights)) {
    console.error(message, name, cause, stack);
    return;
  } else {
    appInsights.trackException({
      error: {
        message: message,
        name: name,
        cause: cause,
        stack: stack,
      },
      severityLevel: 3,
    });
  }
};

export const logInfo = (message: string, name: string): void => {
  if (!hasExternalLoggingService || isNullOrUndefined(appInsights)) {
    console.info(message, name);
    return;
  } else {
    appInsights.trackTrace({
      message: message,
      properties: {
        name: name,
      },
      severityLevel: 0,
    });
  }
};

export const logWarning = (
  message: string,
  name: string,
  error: unknown = undefined
): void => {
  if (!hasExternalLoggingService || isNullOrUndefined(appInsights)) {
    console.warn(message, name, error);
    return;
  } else {
    appInsights.trackTrace({
      message: message,
      properties: {
        name: name,
        error: error,
      },
      severityLevel: 1,
    });
  }
};
