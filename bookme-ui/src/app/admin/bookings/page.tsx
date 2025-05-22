"use client";
import CreateTimeSlotForm from "@/_components/admin/CreateTimeSlotForm";
import { withAuth } from "@/_components/auth/AuthGuard";
import { ROLES } from "@/_lib/enums/constant";
import React from "react";

const BookingAdminPage = () => {
  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-12 ">Bookings</h1>
      <CreateTimeSlotForm />
    </div>
  );
};

export default withAuth(BookingAdminPage, ROLES.ADMIN, {
  fallbackPath: "/unauthorized",
});
