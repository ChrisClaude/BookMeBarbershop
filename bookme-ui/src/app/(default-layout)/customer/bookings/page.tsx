"use client";
import React from "react";
import { withAuth } from "@/_components/auth/AuthGuard";
import Header from "@/_components/header/Header";
import BookingForm from "@/_components/BookingForm";

const BookingsPage = () => {
  return (
    <>
      <Header />
      <section className="py-12 md:py-20 lg:py-24 px-4 md:px-8 lg:px-48">
        <BookingForm />
      </section>
    </>
  );
};

export default withAuth(BookingsPage, "Customer", {
  fallbackPath: "/unauthorized",
});
