'use client';
import { Button, Link } from '@heroui/react';
import React from 'react';
import Logo from './Logo';
import { CONTENT } from '@/_lib/utils/content.utils';
import useLanguageState from '@/_hooks/useLanguageState';

const Header = () => {
  const { language, switchLanguage } = useLanguageState();

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
            <Link href="/">{CONTENT[language].home.services}</Link>
          </li>
          <li>
            <Link href="/">{CONTENT[language].home.services}</Link>
          </li>
          <li>
            <Link href="/">{CONTENT[language].home.contact}</Link>
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
