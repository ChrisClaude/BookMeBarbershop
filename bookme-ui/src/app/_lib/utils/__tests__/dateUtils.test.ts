import { describe, it, expect, beforeEach, afterEach } from "vitest";
import { vi } from "vitest";
import {
  parseDate,
  getDateMonthsFromNow,
  toDisplayDate,
  toDatePickerFormat,
  toShortMonthDisplayDate,
  fromDateOnly,
  getDateYearsAgo,
  getDaysDifference,
  isWithinDays,
  convertDateToUTC,
  compareDates,
} from "../dateUtils";

describe("dateUtils", () => {
  describe("parseDate", () => {
    describe("valid date inputs", () => {
      it.each([
        ["25/12/2023", "2023-12-25T00:00:00.000Z"],
        ["2023-12-25", "2023-12-25T00:00:00.000Z"],
        ["25-12-2023", "2023-12-25T00:00:00.000Z"],
        ["5/6/2023", "2023-06-05T00:00:00.000Z"],
        ["2023-6-5", "2023-06-05T00:00:00.000Z"],
        ["5-6-2023", "2023-06-05T00:00:00.000Z"],
      ])("should parse %s correctly", (input, expected) => {
        const result = parseDate(input);
        expect(result).toBeInstanceOf(Date);
        expect(result?.toISOString()).toBe(expected);
      });

      it("should handle Date object input", () => {
        const date = new Date(Date.UTC(2023, 11, 25));
        const result = parseDate(date);
        expect(result).toBeInstanceOf(Date);
        expect(result?.toISOString()).toBe("2023-12-25T00:00:00.000Z");
      });
    });

    describe("invalid inputs", () => {
      it.each([null, undefined, "", "Choose Date"])(
        "should return null for %s",
        (input) => {
          expect(parseDate(input)).toBeNull();
        }
      );

      it.each([
        ["invalid-date", "Invalid date format"],
        [123, "Invalid date input type"],
        [{} as Date, "Invalid date input type"],
      ])("should throw error for %s with message %s", (input, errorMessage) => {
        expect(() => parseDate(input)).toThrow(errorMessage);
      });
    });
  });

  describe("date formatting functions", () => {
    const testDate = new Date(Date.UTC(2023, 11, 25)); // December 25, 2023

    describe("toDisplayDate", () => {
      it.each([
        [testDate, "25-12-2023"],
        ["2023-12-25", "25-12-2023"],
        [null, ""],
        [undefined, ""],
      ])("should format %s correctly", (input, expected) => {
        expect(toDisplayDate(input)).toBe(expected);
      });

      it("should throw error for invalid date string", () => {
        expect(() => toDisplayDate("invalid-date")).toThrow(
          "Invalid date format"
        );
      });
    });

    describe("toDatePickerFormat", () => {
      it.each([
        [testDate, "25/12/2023"],
        [null, ""],
        [undefined, ""],
      ])("should format %s correctly", (input, expected) => {
        expect(toDatePickerFormat(input)).toBe(expected);
      });
    });

    describe("toShortMonthDisplayDate", () => {
      it.each([
        [testDate, "25-Dec-2023"],
        [null, ""],
        [undefined, ""],
        ["2023-12-25", "25-Dec-2023"],
      ])("should format %s correctly", (input, expected) => {
        expect(toShortMonthDisplayDate(input)).toBe(expected);
      });
    });
  });

  describe("date calculation functions", () => {
    let fixedDate: Date;

    beforeEach(() => {
      // Set the fixed date with explicit UTC time
      fixedDate = new Date(Date.UTC(2025, 0, 24, 12, 0, 0)); // 2025-01-24 12:00:00 UTC
      vi.useFakeTimers();
      vi.setSystemTime(fixedDate);
    });

    afterEach(() => {
      vi.useRealTimers();
    });

    describe("getDateMonthsFromNow", () => {
      it.each([
        [1, "2025-02-24"],
        [3, "2025-04-24"],
        [12, "2026-01-24"],
        [-1, "2024-12-24"],
        [0, "2025-01-24"],
      ])(
        "should calculate %i months from now correctly",
        (months, expected) => {
          const result = getDateMonthsFromNow(months);
          const utcResult = new Date(
            Date.UTC(
              result.getUTCFullYear(),
              result.getUTCMonth(),
              result.getUTCDate(),
              12,
              0,
              0
            )
          );
          expect(utcResult.toISOString().split("T")[0]).toBe(expected);
        }
      );

      it("should handle month end dates correctly", () => {
        vi.setSystemTime(new Date("2025-01-31T00:00:00.000Z"));
        const result = getDateMonthsFromNow(1);
        expect(result.toISOString().split("T")[0]).toBe("2025-02-28");
      });
    });

    describe("getDateYearsAgo", () => {
      it.each([
        [1, "2024-01-24"],
        [5, "2020-01-24"],
        [0, "2025-01-24"],
      ])("should calculate %i years ago correctly", (years, expected) => {
        const result = getDateYearsAgo(years);
        const utcResult = new Date(
          Date.UTC(
            result.getUTCFullYear(),
            result.getUTCMonth(),
            result.getUTCDate(),
            12,
            0,
            0
          )
        );
        expect(utcResult.toISOString().split("T")[0]).toBe(expected);
      });

      it("should throw error for negative years", () => {
        expect(() => getDateYearsAgo(-1)).toThrow(
          "Years must be a positive number"
        );
      });
    });

    describe("getDaysDifference", () => {
      it.each([
        ["2025-01-20", "2025-01-24", 4],
        ["2025-01-24", "2025-01-20", 4],
        ["2025-01-24", null, 0],
        [null, "2025-01-24", 0],
      ])(
        "should calculate difference between %s and %s as %i days",
        (date1, date2, expected) => {
          expect(getDaysDifference(date1, date2)).toBe(expected);
        }
      );
    });
  });

  describe("date comparison functions", () => {
    describe("compareDates", () => {
      const date1 = new Date("2025-01-24");
      const date2 = new Date("2025-01-25");
      const date3 = new Date("2025-01-24");

      it.each([
        [date1, date2, -1],
        [date2, date1, 1],
        [date1, date3, 0],
      ])("should compare dates correctly", (a, b, expected) => {
        expect(compareDates(a, b)).toBe(expected);
      });
    });

    describe("isWithinDays", () => {
      it.each([
        ["2025-01-23", 2, true],
        ["2025-01-27", 2, false],
        [null, 2, false],
      ])(
        "should check if %s is within %i days correctly from %s",
        (date, days, expected) => {
          // Ensure we're using the same time of day for comparison

          const testDate = new Date(date as string);
          testDate.setUTCHours(12, 0, 0, 0);
          expect(isWithinDays(testDate.toISOString(), days, "2025-01-21")).toBe(
            expected
          );
        }
      );
    });
  });

  describe("fromDateOnly", () => {
    it.each([
      [{ year: 2025, month: 1, day: 24 }, true],
      [{ year: 2025, month: 13, day: 24 }, false],
      [{ year: 2025, month: 1, day: 32 }, false],
      [null, false],
      [undefined, false],
      [{}, false],
    ])("should handle input %s correctly", (input, shouldBeValid) => {
      const result = fromDateOnly(input);
      if (shouldBeValid) {
        expect(result).toBeInstanceOf(Date);
      } else {
        expect(result).toBeUndefined();
      }
    });
  });

  describe("convertDateToUTC", () => {
    it("should convert local date to UTC", () => {
      const localDate = new Date(2025, 0, 24, 12, 0, 0);
      const result = convertDateToUTC(localDate);
      expect(result.getUTCHours()).toBe(0);
      expect(result.getUTCMinutes()).toBe(0);
    });

    it("should throw error for invalid date", () => {
      expect(() => convertDateToUTC(new Date("invalid"))).toThrow(
        "Invalid Date object"
      );
    });
  });
});
