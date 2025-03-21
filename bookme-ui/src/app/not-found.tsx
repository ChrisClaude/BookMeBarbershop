'use client';

import React from 'react';
import Link from 'next/link';
import { Button } from '@heroui/react';

const NotFound = () => {
  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-white to-gray-100">
      <div className="text-center space-y-8 p-8">
        <h1 className="text-9xl font-bold text-gray-900">404</h1>
        <div className="space-y-4">
          <h2 className="text-3xl font-semibold text-gray-800">Page Not Found</h2>
          <p className="text-gray-600 max-w-md mx-auto">
            Oops! The page you are looking for might have been removed, had its name
            changed, or is temporarily unavailable.
          </p>
        </div>
        <div className="flex justify-center gap-4">
          <Link href="/">
            <Button color="primary" size="lg">
              Go Home
            </Button>
          </Link>
        </div>
      </div>
    </div>
  );
};

export default NotFound;
