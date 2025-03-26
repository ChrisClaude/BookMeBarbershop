'use client';
import React from 'react';
import { CONTENT } from '@/_lib/utils/content.utils';

const Footer = () => {
  return (
    <footer className="bg-foreground text-background py-8 text-center text-sm">
      <p>
        &copy; {new Date().getFullYear()} {CONTENT.brandName}. All rights
        reserved.
      </p>
    </footer>
  );
};

export default Footer;
