'use client';

import React from 'react';
import Link from 'next/link';
import { Button } from '@heroui/react';
import { useAuth } from '@/_hooks/useAuth';

const UnauthorizedPage = () => {
  const { logout } = useAuth();

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-white to-gray-100">
      <div className="text-center space-y-8 p-8">
        <h1 className="text-9xl font-bold text-red-500">403</h1>
        <div className="space-y-4">
          <h2 className="text-3xl font-semibold text-gray-800">
            Access Denied
          </h2>
          <p className="text-gray-600 max-w-md mx-auto">
            You don&apos;t have permission to access this page. If you believe
            this is a mistake, please contact your administrator or try logging
            in with a different account.
          </p>
        </div>
        <div className="flex justify-center gap-4">
          <Link href="/">
            <Button color="primary" size="lg" className="min-w-[120px]">
              Go Home
            </Button>
          </Link>
          <Button
            color="secondary"
            size="lg"
            className="min-w-[120px]"
            onPress={() => logout()}>
            Sign Out
          </Button>
        </div>
      </div>
    </div>
  );
};

export default UnauthorizedPage;
