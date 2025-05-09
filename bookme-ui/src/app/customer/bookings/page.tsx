"use client";
import React, { useState } from "react";
import { withAuth } from "@/_components/auth/AuthGuard";
import { Button } from "@heroui/react";
import { useAuth } from "@/_hooks/useAuth";
import { UserService } from "@/_lib/services/user.service";

const BookingsPage = () => {
  const { session } = useAuth();
  const [message, setMessage] = useState("");

  const handleBooking = () => {
    UserService.getUserProfile({ token: session?.accessToken }).then((res) => {
      if (res.success && res.data) {
        setMessage(res.data);
      }
    });
  };

  return (
    <>
      <h1>Book your appointment</h1>
      <section>
        <Button onPress={handleBooking}>Book</Button>
        <p>{message}</p>
      </section>
    </>
  );
};

export default withAuth(BookingsPage, "Customer", {
  fallbackPath: "/unauthorized",
});
