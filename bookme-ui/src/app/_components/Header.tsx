'use client';
import React from 'react';
import Logo from './Logo';
import { CONTENT } from '@/_lib/utils/content.utils';
import useLanguageState from '@/_hooks/useLanguageState';
import Link from 'next/link';
import '../../../node_modules/flag-icons/css/flag-icons.min.css';
import { FaFacebookF } from 'react-icons/fa';
import { FaInstagram } from 'react-icons/fa6';

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
      <div className="flex items-center justify-between gap-x-36">
        <div className="flex gap-x-5 items-center">
          <a href="#">
            <FaInstagram size={26} />
          </a>
          <a href="#">
            <FaFacebookF size={26} />
          </a>
        </div>
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
