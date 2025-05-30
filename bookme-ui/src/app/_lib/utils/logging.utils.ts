/**
 * This method provides a wrapper around the error logging mechanism, so that later when an external logging provider is introduced we can do the switch in one place
 * @param message The error message
 * @param name The error name
 * @param cause The error cause
 * @param stack The error stack
 * @returns
 */
export const logError = (message: string, name: string, cause: string | undefined | unknown = undefined, stack: string | undefined = undefined): void => {
	const hasExternalLoggingService = false; // TODO: hardcoded for now - fix this
	if (!hasExternalLoggingService) {
		console.error(message, name, cause, stack);
		return;
	}
};

export const logInfo = (message: string, name: string): void => {
	const hasExternalLoggingService = false; // TODO: hardcoded for now - fix this
	if (!hasExternalLoggingService) {
		console.info(message, name);
		return;
	}
};

export const logWarning = (message: string, name: string, error: unknown = undefined): void => {
	const hasExternalLoggingService = false; // TODO: hardcoded for now - fix this
	if (!hasExternalLoggingService) {
		console.warn(message, name, error);
		return;
	}
};