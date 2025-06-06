import { describe, expect, it } from "vitest";
import {
  isNullOrUndefined,
  isNotNullOrUndefined,
  isNullOrWhiteSpace,
  isNotNullOrWhiteSpace,
  getErrorsFromApiResult,
  binarySearch,
} from "../common.utils";

import { ModelError as ApiError } from "@/_lib/codegen";
import { DateValue, getLocalTimeZone, today } from "@internationalized/date";

describe("common.utils", () => {
  describe("isNullOrUndefined", () => {
    it("should return true for null values", () => {
      expect(isNullOrUndefined(null)).toBe(true);
    });

    it("should return true for undefined values", () => {
      expect(isNullOrUndefined(undefined)).toBe(true);
    });

    it("should return false for non-null values", () => {
      expect(isNullOrUndefined("")).toBe(false);
      expect(isNullOrUndefined(0)).toBe(false);
      expect(isNullOrUndefined(false)).toBe(false);
      expect(isNullOrUndefined({})).toBe(false);
      expect(isNullOrUndefined([])).toBe(false);
    });
  });

  describe("isNotNullOrUndefined", () => {
    it("should return false for null values", () => {
      expect(isNotNullOrUndefined(null)).toBe(false);
    });

    it("should return false for undefined values", () => {
      expect(isNotNullOrUndefined(undefined)).toBe(false);
    });

    it("should return true for non-null values", () => {
      expect(isNotNullOrUndefined("")).toBe(true);
      expect(isNotNullOrUndefined("test")).toBe(true);
      expect(isNotNullOrUndefined(0)).toBe(true);
      expect(isNotNullOrUndefined(false)).toBe(true);
    });
  });

  describe("isNullOrWhiteSpace", () => {
    it("should return true for null values", () => {
      expect(isNullOrWhiteSpace(null)).toBe(true);
    });

    it("should return true for undefined values", () => {
      expect(isNullOrWhiteSpace(undefined)).toBe(true);
    });

    it("should return true for empty strings", () => {
      expect(isNullOrWhiteSpace("")).toBe(true);
    });

    it("should return true for whitespace strings", () => {
      expect(isNullOrWhiteSpace(" ")).toBe(true);
      expect(isNullOrWhiteSpace("  ")).toBe(true);
      expect(isNullOrWhiteSpace("\t")).toBe(true);
      expect(isNullOrWhiteSpace("\n")).toBe(true);
    });

    it("should return false for non-empty strings", () => {
      expect(isNullOrWhiteSpace("test")).toBe(false);
      expect(isNullOrWhiteSpace(" test ")).toBe(false);
    });
  });

  describe("isNotNullOrWhiteSpace", () => {
    it("should return false for null values", () => {
      expect(isNotNullOrWhiteSpace(null)).toBe(false);
    });

    it("should return false for undefined values", () => {
      expect(isNotNullOrWhiteSpace(undefined)).toBe(false);
    });

    it("should return false for empty strings", () => {
      expect(isNotNullOrWhiteSpace("")).toBe(false);
    });

    it("should return false for whitespace strings", () => {
      expect(isNotNullOrWhiteSpace(" ")).toBe(false);
      expect(isNotNullOrWhiteSpace("  ")).toBe(false);
      expect(isNotNullOrWhiteSpace("\t")).toBe(false);
      expect(isNotNullOrWhiteSpace("\n")).toBe(false);
    });

    it("should return true for non-empty strings", () => {
      expect(isNotNullOrWhiteSpace("test")).toBe(true);
      expect(isNotNullOrWhiteSpace(" test ")).toBe(true);
    });
  });

  describe("getErrorsFromApiResult", () => {
    it("should handle array of error objects", () => {
      const result: ApiError[] = [
        {
          code: "Error 1",
          description: "Error 1 description",
        },
        {
          code: "Error 2",
          description: "Error 2 description",
        },
      ];
      expect(getErrorsFromApiResult(result)).toEqual([
        "Error 1 description",
        "Error 2 description",
      ]);
    });

    it("should handle object of errors", () => {
      const result: ApiError[] = [
        {
          code: "Error 1",
          description: "Error 1 description",
        },
        {
          code: "Error 2",
          description: "Error 2 description",
        },
      ];
      expect(getErrorsFromApiResult(result)).toEqual([
        "Error 1 description",
        "Error 2 description",
      ]);
    });

    it("should return default message for invalid error format", () => {
      const result = null;
      //@ts-expect-error: testing invalid input
      expect(getErrorsFromApiResult(result)).toEqual([
        "An unknown error occurred.",
      ]);
    });
  });

  describe("binarySearch", () => {
    it("should return true if target is found", () => {
      const arr: DateValue[] = [];
      const date1 = today(getLocalTimeZone()).add({ days: 5 });
      const date2 = today(getLocalTimeZone()).add({ days: 10 });
      const date3 = today(getLocalTimeZone()).add({ days: 17 });
      const date4 = today(getLocalTimeZone()).add({ days: 19 });
      const date5 = today(getLocalTimeZone()).add({ days: 21 });
      const date6 = today(getLocalTimeZone()).add({ days: 26 });
      arr.push(date1, date2, date3, date4, date5, date6);
      const target = today(getLocalTimeZone()).add({ days: 17 });
      expect(binarySearch(arr, target)).toBe(true);
    });

    it("should return false if target is not found", () => {
      const arr: DateValue[] = [];
      const date1 = today(getLocalTimeZone()).add({ days: 5 });
      const date2 = today(getLocalTimeZone()).add({ days: 10 });
      const date3 = today(getLocalTimeZone()).add({ days: 17 });
      const date4 = today(getLocalTimeZone()).add({ days: 19 });
      const date5 = today(getLocalTimeZone()).add({ days: 21 });
      const date6 = today(getLocalTimeZone()).add({ days: 26 });
      arr.push(date1, date2, date3, date4, date5, date6);
      const target = today(getLocalTimeZone()).add({ days: 20 });
      expect(binarySearch(arr, target)).toBe(false);
    });
  });
});
