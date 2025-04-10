import { logError } from "./logging.utils";

const DATE_FORMATS = {
  API: "yyyy-MM-dd",
  DISPLAY: "dd-MM-yyyy",
  DATEPICKER: "dd/MM/yyyy",
  SHORT_MONTH: "dd-MMM-yyyy",
} as const;

/* Parses a string as a date.  Date format can either by yyyy-MM-dd which is the convention when communicating with the API or dd/MM/yyyy which is the convention
       used within DatePicker.  The function also handles inputs that are type date such that it will return the date itself.
       Thus a call to parseDate(parseDate("19/04/2029")) returns the correct date.
    */
const parseDate = (
  dateString: string | number | Date | null | undefined
): Date | null => {
  if (dateString == null || dateString === "Choose Date") {
    return null;
  }

  if (dateString instanceof Date) {
    return isValidDate(dateString) ? dateString : null;
  }

  if (typeof dateString !== "string") {
    throw new Error(`Invalid date input type: ${typeof dateString}`);
  }

  if (dateString.length === 0) {
    return null;
  }

  const formats = [
    {
      regex: /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/,
      groups: [1, 2, 3],
      monthIndex: 1,
    },
    { regex: /^(\d{4})-(\d{1,2})-(\d{1,2})/, groups: [3, 2, 1], monthIndex: 1 },
    {
      regex: /^(\d{1,2})-(\d{1,2})-(\d{4})$/,
      groups: [1, 2, 3],
      monthIndex: 1,
    },
  ];

  for (const format of formats) {
    const match = dateString.match(format.regex);
    if (match) {
      const [day, month, year] = format.groups.map((g) =>
        parseInt(match[g], 10)
      );
      if (month < 1 || month > 12 || day < 1 || day > 31) {
        continue;
      }
      const date = new Date(Date.UTC(year, month - 1, day));
      if (isValidDate(date)) {
        return date;
      }
    }
  }

  throw new Error(`Invalid date format: ${dateString}`);
};

const fromDatePickerDate = (date: Date): Date | null => {
  /* Hack - DatePicker has a bug that resets the date to today when you press Clear rather than, you known.... clear.
        See here: https://github.com/themesberg/flowbite-react/issues/1366
        Our hack is to clear it if the DatePicker value contains a time component since this should never happen.
        This ensures Clear works at all times except for when clicked on 00:00:00.000 :)
        */
  if (date === undefined || date === null) {
    return date;
  }

  if (
    date.getHours() !== 0 ||
    date.getMinutes() !== 0 ||
    date.getSeconds() !== 0 ||
    date.getMilliseconds() !== 0
  ) {
    return null;
  }

  return date;
};

// Helper function to check if a Date object is valid
const isValidDate = (date: string | Date | null | undefined): date is Date => {
  return date instanceof Date && !isNaN(date.getTime());
};

const today = (): Date => {
  return new Date(
    new Date().getFullYear(),
    new Date().getMonth(),
    new Date().getDate()
  );
};

// Formats a date to 'dd-MM-yyyy' (e.g., 3rd February 2025 => '03-02-2025') to be used when displaying dates to users within the user interface.
const toDisplayDate = (date: string | Date | null | undefined): string => {
  if (date === null) {
    return "";
  }
  if (date === undefined) {
    return "";
  }

  if (typeof date === "string") {
    date = parseDate(date);
  }

  if (!isValidDate(date)) {
    logError(
      `Invalid Date object: ${date} - ${typeof date}`,
      "InvalidDateError"
    );
    return "";
  }

  const day = date.getDate().toString().padStart(2, "0");
  const month = (date.getMonth() + 1).toString().padStart(2, "0");
  const year = date.getFullYear();
  return `${day}-${month}-${year}`;
};

// Formats a date to 'dd/MM/yyyy' (e.g., '03/02/2025') to be used when setting up the value within DatePicker
const toDatePickerFormat = (date: Date | null | undefined): string => {
  if (date == null) {
    return "";
  }

  if (!isValidDate(date)) {
    logError(
      `Invalid Date object: ${date} - ${typeof date}`,
      "InvalidDateError"
    );
    return "";
  }

  const day = date.getDate().toString().padStart(2, "0");
  const month = (date.getMonth() + 1).toString().padStart(2, "0"); // Months are zero-indexed
  const year = date.getFullYear();
  return `${day}/${month}/${year}`;
};

const convertDateToUTC = (date: Date): Date => {
  if (!(date instanceof Date) || isNaN(date.getTime())) {
    throw new Error("Invalid Date object provided to convertDateToUTC.");
  }

  return new Date(
    Date.UTC(date.getFullYear(), date.getMonth(), date.getDate())
  );
};

// Formats a date to 'dd-MMM-yyyy' (e.g., 3rd February 2025 => '03-Feb-2025') to be used for displaying dates to users.
const toShortMonthDisplayDate = (
  date: string | Date | null | undefined
): string => {
  if (date === null || date === undefined) {
    return "";
  }

  if (typeof date === "string") {
    date = parseDate(date);
  }

  if (!isValidDate(date)) {
    logError(
      `Invalid Date object: ${date} - ${typeof date}`,
      "InvalidDateError"
    );
    return "";
  }

  const day = date.getDate().toString().padStart(2, "0");
  const month = date.toLocaleString("en", { month: "short" });
  const year = date.getFullYear();
  return `${day}-${month}-${year}`;
};

/**
 * Converts a DateOnly-like object (with year, month, day properties) to a Date.
 * If any of the properties are missing or invalid, returns null.
 *
 * @param dateOnly - DateOnly object without time
 * @returns {Date | null} - A Date object if conversion is successful; otherwise, null.
 */
const fromDateOnly = (
  dateOnly: { year?: number; month?: number; day?: number } | null | undefined
): Date | undefined => {
  if (!dateOnly) {
    return undefined;
  }

  const { year, month, day } = dateOnly;

  if (year === undefined || month === undefined || day === undefined) {
    logError(
      `Missing required date components: ${JSON.stringify(dateOnly)}`,
      "InvalidDateOnlyError"
    );
    return undefined;
  }

  if (month < 1 || month > 12 || day < 1 || day > 31 || year < 1900) {
    logError(
      `Invalid date components: year=${year}, month=${month}, day=${day}`,
      "InvalidDateOnlyError"
    );
    return undefined;
  }

  const date = new Date(year, month - 1, day);
  return isValidDate(date) ? date : undefined;
};

/**
 * Returns a Date object that is the current date minus the number of years specified.
 *
 * @param years - The number of years to subtract from the current date.
 * @returns A Date object representing the date years ago.
 */
const getDateYearsAgo = (years: number): Date => {
  if (years < 0) {
    throw new Error("Years must be a positive number");
  }
  const today = new Date();
  return new Date(
    today.getFullYear() - years,
    today.getMonth(),
    today.getDate()
  );
};

/**
 * Calculates the number of days between two dates
 *
 * @param date1 - First date for comparison
 * @param date2 - Second date for comparison (defaults to today if not provided)
 * @returns number of days difference between the dates
 */
const getDaysDifference = (
  date1: Date | string | null,
  date2: Date | string | null = today()
): number => {
  // Handle null/invalid inputs
  if (!date1 || !date2) {
    return 0;
  }

  // Parse dates if they're strings
  const parsedDate1 = typeof date1 === "string" ? parseDate(date1) : date1;
  const parsedDate2 = typeof date2 === "string" ? parseDate(date2) : date2;

  if (!parsedDate1 || !parsedDate2) {
    return 0;
  }

  // Calculate difference in days
  const diffTime = Math.abs(parsedDate2.getTime() - parsedDate1.getTime());
  return Math.floor(diffTime / (1000 * 60 * 60 * 24));
};

/**
 * Checks if a date is within a specified number of days from another date
 *
 * @param date - The date to check
 * @param days - Number of days to check against
 * @param fromDate - The date to check from (defaults to today)
 * @returns boolean indicating if the date is within the specified number of days
 */
const isWithinDays = (
  date: Date | string | null,
  days: number,
  fromDate: Date | string | null = today()
): boolean => {
  return getDaysDifference(date, fromDate) < days;
};

/**
 * Checks if a date is not within a specified number of days from another date
 *
 * @param date - The date to check
 * @param days - Number of days to check against
 * @param fromDate - The date to check from (defaults to today)
 * @returns boolean indicating if the date is not within the specified number of days
 */
const isNotWithinDays = (
  date: Date | string | null,
  days: number,
  fromDate: Date | string | null = today()
): boolean => {
  return !(getDaysDifference(date, fromDate) < days);
};

/**
 * Gets a date that is a specified number of months from now
 * @param months - Number of months to add to current date
 * @returns Date object representing the future date
 */
const getDateMonthsFromNow = (months: number): Date => {
  const date = new Date();
  const currentDate = date.getDate();

  // Set to first day of month to avoid date rollover
  date.setDate(1);
  date.setMonth(date.getMonth() + months);

  // Get last day of target month
  const lastDay = new Date(
    date.getFullYear(),
    date.getMonth() + 1,
    0
  ).getDate();

  // Set to original date or last day of month, whichever is smaller
  date.setDate(Math.min(currentDate, lastDay));

  return date;
};

const compareDates = (date1: Date, date2: Date): -1 | 0 | 1 => {
  const d1 = new Date(date1.getFullYear(), date1.getMonth(), date1.getDate());
  const d2 = new Date(date2.getFullYear(), date2.getMonth(), date2.getDate());

  if (d1 < d2) return -1;
  if (d1 > d2) return 1;
  return 0;
};

export {
  parseDate,
  fromDatePickerDate,
  toDisplayDate,
  toDatePickerFormat,
  toShortMonthDisplayDate,
  fromDateOnly,
  getDateYearsAgo,
  getDaysDifference,
  isWithinDays,
  isNotWithinDays,
  getDateMonthsFromNow,
  convertDateToUTC,
  compareDates,
};
