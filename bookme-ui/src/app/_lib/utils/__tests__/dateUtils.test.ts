import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import { vi } from 'vitest';
import { parseDate, getDateMonthsFromNow } from '../dateUtils';

describe('parseDate', () => {

  describe('valid date inputs', () => {
    it('should parse dd/MM/yyyy format correctly', () => {
      const result = parseDate('25/12/2023');
      expect(result).toBeInstanceOf(Date);
      expect(result?.toISOString()).toBe('2023-12-25T00:00:00.000Z');
    });

    it('should parse yyyy-MM-dd format correctly', () => {
      const result = parseDate('2023-12-25');
      expect(result).toBeInstanceOf(Date);
      expect(result?.toISOString()).toBe('2023-12-25T00:00:00.000Z');
    });

    it('should parse yyyy-MM-dd format correctly with time zone', () => {
      const result = parseDate('2025-03-01T00:00:00+00:00');
      expect(result).toBeInstanceOf(Date);
      expect(result?.toISOString()).toBe('2025-03-01T00:00:00.000Z');
    });

    it('should parse dd-MM-yyyy format correctly', () => {
      const result = parseDate('25-12-2023');
      expect(result).toBeInstanceOf(Date);
      expect(result?.toISOString()).toBe('2023-12-25T00:00:00.000Z');
    });

    it('should handle Date object input', () => {
      const date = new Date(Date.UTC(2023, 11, 25)); // December 25, 2023
      const result = parseDate(date);
      expect(result).toBeInstanceOf(Date);
      expect(result?.toISOString()).toBe('2023-12-25T00:00:00.000Z');
    });
  });

  describe('null or empty inputs', () => {
    it('should return null for null input', () => {
      expect(parseDate(null)).toBeNull();
    });

    it('should return null for undefined input', () => {
      expect(parseDate(undefined)).toBeNull();
    });

    it('should return null for empty string', () => {
      expect(parseDate('')).toBeNull();
    });

    it('should return null for "Choose Date" string', () => {
      expect(parseDate('Choose Date')).toBeNull();
    });
  });

  describe('invalid inputs', () => {
    it('should throw error for invalid date string format', () => {
      expect(() => parseDate('invalid-date')).toThrow('Invalid date format');
    });

    // TODO: We have to cater for scenarios where the date is invalid and return an error
    // it('should throw error for invalid date values', () => {
    //   expect(() => parseDate('32/13/2023')).toThrow('Invalid date format');
    // });

    it('should throw error for invalid input types', () => {
      expect(() => parseDate(123)).toThrow('Invalid date input type');
      expect(() => parseDate({} as Date)).toThrow('Invalid date input type');
    });
  });

  describe('edge cases', () => {
    it('should handle single digit days and months in dd/MM/yyyy format', () => {
      const result = parseDate('5/6/2023');
      expect(result).toBeInstanceOf(Date);
      expect(result?.toISOString()).toBe('2023-06-05T00:00:00.000Z');
    });

    it('should handle single digit days and months in yyyy-MM-dd format', () => {
      const result = parseDate('2023-6-5');
      expect(result).toBeInstanceOf(Date);
      expect(result?.toISOString()).toBe('2023-06-05T00:00:00.000Z');
    });

    it('should handle single digit days and months in dd-MM-yyyy format', () => {
      const result = parseDate('5-6-2023');
      expect(result).toBeInstanceOf(Date);
      expect(result?.toISOString()).toBe('2023-06-05T00:00:00.000Z');
    });
  });

  describe('getDateMonthsFromNow', () => {
    beforeEach(() => {
      // Mock the current date to 2025-01-24
      vi.useFakeTimers();
      vi.setSystemTime(new Date('2025-01-24T23:15:23+01:00'));
    });

    afterEach(() => {
      vi.useRealTimers();
    });

    it('should return a date 1 month from now', () => {
      const result = getDateMonthsFromNow(1);
      expect(result.getFullYear()).toBe(2025);
      expect(result.getMonth()).toBe(1); // February (0-based)
      expect(result.getDate()).toBe(24);
    });

    it('should return a date 3 months from now', () => {
      const result = getDateMonthsFromNow(3);
      expect(result.getFullYear()).toBe(2025);
      expect(result.getMonth()).toBe(3); // April (0-based)
      expect(result.getDate()).toBe(24);
    });

    it('should handle year rollover', () => {
      const result = getDateMonthsFromNow(12);
      expect(result.getFullYear()).toBe(2026);
      expect(result.getMonth()).toBe(0); // January (0-based)
      expect(result.getDate()).toBe(24);
    });

    it('should handle negative months', () => {
      const result = getDateMonthsFromNow(-1);
      expect(result.getFullYear()).toBe(2024);
      expect(result.getMonth()).toBe(11); // December (0-based)
      expect(result.getDate()).toBe(24);
    });

    it('should handle zero months', () => {
      const result = getDateMonthsFromNow(0);
      expect(result.getFullYear()).toBe(2025);
      expect(result.getMonth()).toBe(0); // January (0-based)
      expect(result.getDate()).toBe(24);
    });

    it('should handle month end dates by capping at last day of target month', () => {
      vi.setSystemTime(new Date('2025-01-31T00:00:00.000Z'));
      const result = getDateMonthsFromNow(1);
      expect(result.getFullYear()).toBe(2025);
      expect(result.getMonth()).toBe(1); // February
      expect(result.getDate()).toBe(28); // Last day of February 2025
    });

    it('should preserve date when target month has enough days', () => {
      vi.setSystemTime(new Date('2025-01-28T00:00:00.000Z'));
      const result = getDateMonthsFromNow(1);
      expect(result.getFullYear()).toBe(2025);
      expect(result.getMonth()).toBe(1); // February
      expect(result.getDate()).toBe(28); // Preserves the 28th
    });
  });
});