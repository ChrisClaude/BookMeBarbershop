import { UserType } from "../types";
import { BookingStatusType } from "../types/common.types";

export const ROLES: Record<"ADMIN" | "CUSTOMER", UserType> = {
  ADMIN: "Admin",
  CUSTOMER: "Customer",
};

export const BOOKING_STATUS : Record<string, BookingStatusType> = {
  PENDING: "Pending",
  CONFIRMED: "Confirmed",
  CANCELLED: "Cancelled",
  COMPLETED: "Completed",
};

export const bookingStatusToNumber = (status: string): number => {
  switch (status) {
    case "Pending":
      return 0;
    case "Confirmed":
      return 1;
    case "Cancelled":
      return 2;
    case "Completed":
      return 3;
    default:
      throw new Error("Invalid booking status");
  }
};

export const bookingStatusToString = (status: number | null | undefined) : BookingStatusType => {
  switch (status) {
    case 0:
      return "Pending";
    case 1:
      return "Confirmed";
    case 2:
      return "Cancelled";
    case 3:
      return "Completed";
    default:
      throw new Error("Invalid booking status");
  }
};

export const localLinks = {
  admin: {
    dashboard: "/admin",
    users: "/admin/users",
    bookings: "/admin/bookings",
  },
  customer: {
    bookingAppointment: "/customer/book-appointment",
    bookingList: "/customer/booking-list",
    profile: "/customer/profile",
  },
};
