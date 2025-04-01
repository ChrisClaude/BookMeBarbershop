'use client';

import React, { ReactNode, useEffect, useState } from 'react';
import { usePathname, useRouter } from 'next/navigation';
import Loader from '../Loader';
import { useAuth } from '@/_hooks/useAuth';
import { UserType } from '@/_lib/types';
import { logError } from '@/_lib/utils/logging.utils';

type AuthGuardProps = {
  userType: UserType;
  children: ReactNode;
  fallbackPath?: string;
  loadingComponent?: ReactNode;
}

export const AuthGuard: React.FC<AuthGuardProps> = ({
  userType,
  children,
  fallbackPath = '/',
  loadingComponent = <Loader />,
}) => {
  const { status, userProfile, login } = useAuth();
  const router = useRouter();
  const pathname = usePathname();
  const [isAuthorized, setIsAuthorized] = useState<boolean>(false);
  const [isLoading, setIsLoading] = useState<boolean>(true);

  useEffect(() => {
    const handleAuth = async () => {
      try {
        switch (status) {
          case 'loading':
            return;

          case 'unauthenticated':
            // Store the intended destination before redirecting to login
            sessionStorage.setItem('returnUrl', pathname);
            login();
            return;

          case 'authenticated':
            if (!userProfile) {
              throw new Error('User profile is undefined');
            }

            if (userProfile.userType !== userType) {
              router.replace(fallbackPath);
              return;
            }

            setIsAuthorized(true);
            // Restore the intended destination after successful auth
            const returnUrl = sessionStorage.getItem('returnUrl');
            if (returnUrl && returnUrl !== pathname) {
              router.replace(returnUrl);
              sessionStorage.removeItem('returnUrl');
            }
            break;

          default:
            throw new Error(`Unexpected auth status: ${status}`);
        }
      } catch (error) {
        logError('AuthGuard error', 'AuthGuard', error);
        router.replace('/error');
      } finally {
        setIsLoading(false);
      }
    };

    handleAuth();
  }, [status, userProfile, userType, router, pathname, login, fallbackPath]);

  // Show loading state
  if (isLoading || status === 'loading' || !userProfile || !isAuthorized) {
    return loadingComponent;
  }

  // Render children only when authorized
  return <>{children}</>;
};

// HOC for wrapping components that need authentication
export const withAuth = <P extends object>(
  WrappedComponent: React.ComponentType<P>,
  userType: UserType,
  options: Partial<Omit<AuthGuardProps, 'userType' | 'children'>> = {}
) => {
  return function WithAuthComponent(props: P) {
    return (
      <AuthGuard userType={userType} {...options}>
        <WrappedComponent {...props} />
      </AuthGuard>
    );
  };
};
