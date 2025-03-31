'use client';

import React, { ReactNode, useEffect, useState } from 'react';
import { usePathname, useRouter } from 'next/navigation';
import { signIn } from 'next-auth/react';
import Loader from '../Loader';
import { useAuth } from '@/_hooks/useAuth';

interface AuthGuardProps {
  userType: 'Candidate' | 'Client' | 'Admin';
  children: ReactNode;
}

export const AuthGuard: React.FC<AuthGuardProps> = ({ userType, children }) => {
  const { status, profile: userProfile } = useAuth();
  const router = useRouter();
  const pathname = usePathname();
  const [isAuthorized, setIsAuthorized] = useState(false);

  useEffect(() => {
    if (status === 'loading') {
      return;
    }

    if (status === 'unauthenticated') {
      signIn(pathname, { callbackUrl: pathname });
      return;
    }

    if (userProfile === undefined) {
      return;
    }

    if (userProfile.userType !== userType) {
      router.push('/');
    } else {
      setIsAuthorized(true);
    }
  }, [status, userProfile, userType, router, pathname]);

  if (status === 'loading' || userProfile === undefined || !isAuthorized) {
    return <Loader />;
  }

  return <>{children}</>;
};
