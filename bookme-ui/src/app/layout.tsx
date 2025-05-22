import type { Metadata } from "next";
import "./globals.css";
import Script from "next/script";
import Providers from "./_components/Providers";
import { COOKIE_BOT_DOMAIN_GROUP_ID } from "./config";

import { Montserrat, Open_Sans } from "next/font/google";
import SessionProviderWrapper from "./_components/auth/SessionProviderWrapper";

const montserrat = Montserrat({
  subsets: ["latin"],
  display: "swap",
});

const openSans = Open_Sans({
  subsets: ["latin"],
  display: "swap",
});

export const metadata: Metadata = {
  title: "BookMe",
  description: "Book your next haircut today!",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="en"
      suppressContentEditableWarning={true}
      className={montserrat.className}
    >
      <Script
        src={`https://consent.cookiebot.com/uc.js?cbid=${COOKIE_BOT_DOMAIN_GROUP_ID}`}
      />
      <body className={openSans.className}>
        <Providers>
          <SessionProviderWrapper>{children}</SessionProviderWrapper>
        </Providers>
      </body>
    </html>
  );
}
