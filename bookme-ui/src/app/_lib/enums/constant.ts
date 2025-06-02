import { UserType } from "../types";

export const ROLES : Record<"ADMIN" | "CUSTOMER", UserType> = {
  ADMIN: "Admin",
  CUSTOMER: "Customer",
} ;

export const BOOKING_STATUS = {
  PENDING: "Pending",
  CONFIRMED: "Confirmed",
  CANCELLED: "Cancelled",
  COMPLETED: "Completed",
};

export const bookingStatusToNumber = (status: string) => {
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
