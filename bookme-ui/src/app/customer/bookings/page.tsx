'use client';

import React from 'react';
import { withAuth } from '@/_components/auth/AuthGuard';

const BookingsPage = () => {
  return <div>BookingsPage</div>;
};

export default withAuth(BookingsPage, 'Customer', {
  fallbackPath: '/unauthorized',
});
