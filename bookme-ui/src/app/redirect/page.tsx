"use client";
import React from "react";
import { logInfo } from "@/_lib/utils/logging.utils";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/_hooks/useAuth";
import { Spinner } from "@heroui/react";
import { localLinks } from "@/_lib/enums/constant";

const Page = () => {
  const router = useRouter();
  const { status, isAdmin, isCustomer, isFetchingUserProfile } = useAuth();
  const [redirectMessage, setRedirectMessage] = useState(
    "Redirecting you to the appropriate page..."
  );

  useEffect(() => {
    logInfo("You are being redirected", "RedirectPage");

    if (status === "loading" || isFetchingUserProfile) {
      setRedirectMessage("Loading...");
      return;
    }

    let destination = "/";

    if (status === "authenticated") {
      if (isAdmin) {
        destination = localLinks.admin.dashboard;
        setRedirectMessage("Redirecting you to admin dashboard...");
      } else if (isCustomer) {
        destination = localLinks.customer.bookingList;
        setRedirectMessage("Redirecting you to your bookings...");
      } else {
        destination = "/unauthorized";
        setRedirectMessage("You don't have the required permissions...");
      }
    } else {
      setRedirectMessage("Redirecting you to the homepage...");
    }

    const redirectTimer = setTimeout(() => {
      router.push(destination);
    }, 1500);

    return () => clearTimeout(redirectTimer);
  }, [isAdmin, isCustomer, router, status, isFetchingUserProfile]);

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gradient-to-b from-white to-gray-100">
      <div className="text-center space-y-8 p-8 max-w-md">
        <div className="flex justify-center">
          <Spinner size="lg" color="primary" className="h-16 w-16" />
        </div>
        <h1 className="text-3xl font-bold text-gray-800">Just a moment</h1>
        <p className="text-gray-600">{redirectMessage}</p>
        <div className="w-full bg-gray-200 rounded-full h-2.5 mt-4">
          <div className="bg-accent h-2.5 rounded-full animate-[progress_1.5s_ease-in-out_infinite]"></div>
        </div>
      </div>
    </div>
  );
};

export default Page;
