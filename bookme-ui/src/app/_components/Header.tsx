'use client';
import { Link } from '@heroui/react';
import React from 'react';
import Logo from './Logo';
import { CONTENT } from '@/utils/content.utils';

const Header = () => {
  return (
    <header className="flex justify-between items-center p-4">
      <div className="text-2xl font-bold">
        <Link href="/">
          <Logo />
        </Link>
      </div>
      <nav>
        <ul className="flex gap-6">
          <li>
            <Link href="/">{CONTENT.en.home.services}</Link>
          </li>
          <li>
            <Link href="/">{CONTENT.en.home.services}</Link>
          </li>
          <li>
            <Link href="/">{CONTENT.en.home.services}</Link>
          </li>
        </ul>
      </nav>
    </header>
  );
};

export default Header;
