"use client";
import { withAuth } from '@/_components/auth/AuthGuard';
import React from 'react';

const AdminPage = () => {
  return <div>AdminPage</div>;
};

export default withAuth(AdminPage, "Customer", {
  fallbackPath: "/unauthorized",
});
