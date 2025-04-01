import { withAuth } from '@/_components/auth/AuthGuard';
import React from 'react';

const UserProfilePage = () => {
  return <div>UserProfilePage</div>;
};

export default withAuth(UserProfilePage, 'Customer', {
  fallbackPath: '/unauthorized',
});
