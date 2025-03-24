'use client';
import { SessionProvider } from 'next-auth/react';
import React from 'react';
import { HeroUIProvider } from '@heroui/react';
import StoreProvider from './StoreProvider';

const AppClientComponentWrapper = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  return (
    <StoreProvider>
      <HeroUIProvider>
        <SessionProvider>{children}</SessionProvider>
      </HeroUIProvider>
    </StoreProvider>
  );
};

export default AppClientComponentWrapper;
