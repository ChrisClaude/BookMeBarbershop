'use client';
import { SessionProvider } from 'next-auth/react';
import React from 'react';
import { HeroUIProvider } from '@heroui/react';
import StoreProvider from './StoreProvider';

const Providers = ({ children }: { children: React.ReactNode }) => {
  return (
    <HeroUIProvider>
      <StoreProvider>
        <SessionProvider>{children}</SessionProvider>
      </StoreProvider>
    </HeroUIProvider>
  );
};

export default Providers;
