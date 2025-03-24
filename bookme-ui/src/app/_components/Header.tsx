'use client';
import { Link } from '@heroui/react';
import React from 'react';
import Logo from './Logo';

const Header = () => {
  return (
    <header className="flex justify-between items-center p-4">
      <nav>
        <ul className="flex gap-6">
          <li>
            <Link href="/">Templates</Link>
          </li>
          <li>
            <Link href="/">About</Link>
          </li>
        </ul>
      </nav>
      <div className="text-2xl font-bold">
        <Link href="/">
          <Logo />
        </Link>
      </div>
    </header>
  );
};

export default Header;
