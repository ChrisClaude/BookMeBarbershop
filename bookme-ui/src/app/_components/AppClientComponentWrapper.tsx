"use client";
import { SessionProvider } from 'next-auth/react';
import React from 'react';
import { HeroUIProvider } from '@heroui/react';

const AppClientComponentWrapper = ({children}: {children: React.ReactNode}) => {
  return   <HeroUIProvider>
    <SessionProvider>{children}</SessionProvider>
  </HeroUIProvider>;
};

export default AppClientComponentWrapper;
