"use client";
import { withAuth } from '@/_components/auth/AuthGuard';
import { ROLES } from '@/_lib/enums/constant';
import React from 'react';

const AdminPage = () => {
  return <div>AdminPage</div>;
};

export default withAuth(AdminPage, ROLES.ADMIN, {
  fallbackPath: "/unauthorized",
});
