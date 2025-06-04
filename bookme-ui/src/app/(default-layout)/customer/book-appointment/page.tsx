"use client";
import React from "react";
import { withAuth } from "@/_components/auth/AuthGuard";
import BookingForm from "@/_components/booking-form/BookingForm";
import { ROLES } from "@/_lib/enums/constant";
import Header from "@/_components/header/Header";

const BookAppointmentPage = () => {
  return (
    <>
      <Header />
      <section className="py-12 md:py-20 lg:py-24 px-4 md:px-8 lg:px-48">
        <BookingForm />
      </section>
    </>
  );
};

export default withAuth(BookAppointmentPage, ROLES.CUSTOMER, {
  fallbackPath: "/unauthorized",
});
