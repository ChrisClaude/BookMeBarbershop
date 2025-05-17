"use client";
import { withAuth } from "@/_components/auth/AuthGuard";
import CreateTimeSlotForm from "@/_components/CreateTimeSlotForm";
import React from "react";

const BookingAdminPage = () => {
  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-12 ">Bookings</h1>
      <CreateTimeSlotForm />
    </div>
  );
};

export default withAuth(BookingAdminPage, "Customer", {
  fallbackPath: "/unauthorized",
});
