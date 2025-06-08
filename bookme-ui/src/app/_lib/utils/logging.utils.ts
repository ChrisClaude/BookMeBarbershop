import { appInsights } from "@/appInsights";
import { isNotNullOrWhiteSpace, isNullOrUndefined } from "./common.utils";
import { APP_INSIGHTS_CONNECTION_STRING } from "@/config";

const hasExternalLoggingService = isNotNullOrWhiteSpace(
  APP_INSIGHTS_CONNECTION_STRING
);

const SeverityLevel = {
  VERBOSE: 0,
  INFO: 1,
  WARNING: 2,
  ERROR: 3,
  CRITICAL: 4,
};

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
      severityLevel: SeverityLevel.ERROR,
    });
  }
};

export const logInfo = (
  message: string,
  name: string,
  properties?: Record<string, unknown>
): void => {
  if (!hasExternalLoggingService || isNullOrUndefined(appInsights)) {
    console.info(message, name, properties);
    return;
  } else {
    appInsights.trackTrace({
      message: message,
      properties: {
        name: name,
        ...properties,
      },
      severityLevel: SeverityLevel.INFO,
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
      severityLevel: SeverityLevel.WARNING,
    });
  }
};
