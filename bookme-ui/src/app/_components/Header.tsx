'use client';
import { Button } from '@heroui/react';
import React from 'react';
import Logo from './Logo';
import { CONTENT } from '@/_lib/utils/content.utils';
import useLanguageState from '@/_hooks/useLanguageState';
import Link from 'next/link';

const Header = () => {
  const { language, switchLanguage } = useLanguageState();

  return (
    <header className="flex justify-between items-center py-6 px-48">
      <div>
        <Link href="/">
          <Logo />
        </Link>
      </div>
      <nav>
        <ul className="flex gap-6 uppercase">
          <li>
            <Link
              href="/"
              className="nav-link">
              {CONTENT[language].home.services}
            </Link>
          </li>
          <li>
            <Link
              href="/"
              className="nav-link">
              {CONTENT[language].home.gallery}
            </Link>
          </li>
          <li>
            <Link href="/" className='nav-link'>{CONTENT[language].home.contact}</Link>
          </li>
        </ul>
      </nav>
      <Button
        color="primary"
        onPress={() => switchLanguage(language === 'en' ? 'pl' : 'en')}>
        {language.toUpperCase()}
      </Button>
    </header>
  );
};

export default Header;
