'use client';
import React, { useEffect } from 'react';
import { signIn, signOut, useSession } from 'next-auth/react';
import { isPast } from 'date-fns';

const SessionRefreshProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const { data: session, status } = useSession();

  useEffect(() => {
    const refreshToken = () => {
      // @ts-expect-error we augmented the session object with error in api/auth/[...nextauth]/route.ts
      if (session?.error === "RefreshAccessTokenError" || session?.error === "RefreshTokenExpired"|| session?.error === "RefreshTokenNotSet") {
        signIn('azure-ad-b2c'); // Force sign in to hopefully resolve error
        return;
      }

      // @ts-expect-error we augmented the session object with accessToken and refreshToken in api/auth/[...nextauth]/route.ts
      if (session && session.user && (!session.accessToken && !session.refreshToken)) {
        signOut();
        return;
      }

      // @ts-expect-error we augmented the session object with accessTokenExpires and refreshTokenExpires in api/auth/[...nextauth]/route.ts
      if (session?.accessToken && !session?.accessTokenExpires || isPast(session?.accessTokenExpires)) {
        // @ts-expect-error we augmented the session object with refreshToken and refreshTokenExpires in api/auth/[...nextauth]/route.ts
        if (!session.refreshToken || !session.refreshTokenExpires || session?.refreshTokenExpires && isPast(session?.refreshTokenExpires)) {
          signOut();
        }
      }
    };

    refreshToken();

  }, [session, status]);


  return <>{children}</>;
};

export default SessionRefreshProvider;