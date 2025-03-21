'use client'; // Error boundaries must be Client Components

import React from 'react';
import { useEffect } from 'react';
import { logError } from './utils/logging.utils';
import { Button } from '@heroui/react';

export default function Error({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  useEffect(() => {
    logError(error.message, error.name, error.cause, error.stack);
  }, [error]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-b from-white to-gray-100">
      <div className="text-center space-y-8 p-8">
        <div className="text-6xl font-bold text-red-500">⚠️</div>
        <div className="space-y-4">
          <h2 className="text-3xl font-semibold text-gray-800">Something went wrong!</h2>
          <p className="text-gray-600 max-w-md mx-auto">
            We apologize for the inconvenience. Please try again, and if the problem persists,
            contact our support team.
          </p>
        </div>
        <div className="flex justify-center gap-4">
          <Button
            color="primary"
            size="lg"
            onPress={() => reset()}>
            Try Again
          </Button>
        </div>
      </div>
    </div>
  );
}
