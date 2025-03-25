'use client';
import { Button } from '@heroui/react';
import React from 'react';
import Logo from './Logo';
import { CONTENT } from '@/_lib/utils/content.utils';
import useLanguageState from '@/_hooks/useLanguageState';
import Link from 'next/link';
import '../../../node_modules/flag-icons/css/flag-icons.min.css';

const Header = () => {
  const { language, switchLanguage } = useLanguageState();

  return (
    <header className="flex justify-between items-center py-6 px-48 banner-bg">
      <div>
        <Link href="/">
          <Logo />
        </Link>
      </div>
      <nav>
        <ul className="flex gap-6 uppercase">
          <li>
            <Link href="/" className="nav-link">
              {CONTENT[language].home.services}
            </Link>
          </li>
          <li>
            <Link href="/" className="nav-link">
              {CONTENT[language].home.gallery}
            </Link>
          </li>
          <li>
            <Link href="/" className="nav-link">
              {CONTENT[language].home.contact}
            </Link>
          </li>
        </ul>
      </nav>
      <div>
        <div>
          <button
            onClick={() => switchLanguage(language === 'en' ? 'pl' : 'en')}>
            <span
              className={`fi fi-${language === 'en' ? 'gb' : 'pl'} fis h-1`}
              style={{ height: '27px', width: '38px' }}></span>
          </button>
        </div>
      </div>
    </header>
  );
};

export default Header;
