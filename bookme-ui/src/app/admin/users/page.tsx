"use client";
import { withAuth } from '@/_components/auth/AuthGuard';
import { ROLES } from '@/_lib/enums/constant';
import React from 'react';

const UserAdminPage = () => {
  return <div>User AdminPage</div>;
};

export default withAuth(UserAdminPage, ROLES.ADMIN, {
  fallbackPath: "/unauthorized",
});
