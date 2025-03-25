import type { Metadata } from 'next';
import './globals.css';
import Script from 'next/script';
import Providers from './_components/Providers';
import { COOKIE_BOT_DOMAIN_GROUP_ID } from './config';

import { Montserrat } from 'next/font/google';

const montserrat = Montserrat({
  subsets: ['latin'],
});

export const metadata: Metadata = {
  title: 'BookMe',
  description: 'Book your next haircut today!',
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
      className={montserrat.className}>
      <Script
        src={`https://consent.cookiebot.com/uc.js?cbid=${COOKIE_BOT_DOMAIN_GROUP_ID}`}
      />
      <body>
        <Providers>{children}</Providers>
      </body>
    </html>
  );
}
