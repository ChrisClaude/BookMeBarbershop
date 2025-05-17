"use client";
import { withAuth } from '@/_components/auth/AuthGuard';
import React from 'react';

const UserAdminPage = () => {
  return <div>User AdminPage</div>;
};

export default withAuth(UserAdminPage, "Customer", {
  fallbackPath: "/unauthorized",
});
