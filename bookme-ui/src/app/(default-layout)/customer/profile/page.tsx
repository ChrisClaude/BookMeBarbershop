"use client";
import { withAuth } from "@/_components/auth/AuthGuard";
import { ROLES } from "@/_lib/enums/constant";
import React from "react";
import ProfileForm from "@/_components/profile-form/ProfileForm";

const ProfilePage = () => {

  return (
  <ProfileForm />
  );
};

export default withAuth(ProfilePage, ROLES.CUSTOMER, {
  fallbackPath: "/unauthorized",
});
