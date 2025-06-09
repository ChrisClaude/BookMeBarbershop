"use client";
import { SessionProvider } from "next-auth/react";
import React from "react";
import { HeroUIProvider, ToastProvider } from "@heroui/react";
import StoreProvider from "./StoreProvider";

const Providers = ({ children }: { children: React.ReactNode }) => {
  return (
    <HeroUIProvider>
      <ToastProvider />
      <StoreProvider>
        <SessionProvider>{children}</SessionProvider>
      </StoreProvider>
    </HeroUIProvider>
  );
};

export default Providers;
