'use client';
import React from 'react';
import { CONTENT } from '@/_lib/utils/content.utils';
import { twMerge } from 'tailwind-merge';

const Footer = ({className}: { className?: string }) => {
  return (
    <footer className={twMerge("bg-foreground text-background py-8 text-center text-sm", className)}>
      <p>
        &copy; {new Date().getFullYear()} {CONTENT.brandName}. All rights
        reserved.
      </p>
    </footer>
  );
};

export default Footer;
