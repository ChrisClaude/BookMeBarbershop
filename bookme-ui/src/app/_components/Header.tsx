'use client';
import React from 'react';
import Logo from './Logo';
import { CONTENT } from '@/_lib/utils/content.utils';
import useLanguageState from '@/_hooks/useLanguageState';
import Link from 'next/link';
import '../../../node_modules/flag-icons/css/flag-icons.min.css';
import { FaInstagram } from 'react-icons/fa6';
import Image from 'next/image';

const Header = () => {
  const { language } = useLanguageState();

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
            <Link href="#services" className="nav-link">
              {CONTENT[language].home.services}
            </Link>
          </li>
          <li>
            <Link href="#gallery" className="nav-link">
              {CONTENT[language].home.gallery}
            </Link>
          </li>
          <li>
            <Link href="#contact" className="nav-link">
              {CONTENT[language].home.contact}
            </Link>
          </li>
        </ul>
      </nav>
      <div className="flex items-center justify-between gap-x-36">
        <div className="flex gap-x-5 items-center">
          {/* <a href="#">
            <FaFacebookF size={26} />
          </a> */}
          <a
            href="https://www.instagram.com/sans_tache_papi?utm_source=ig_web_button_share_sheet&igsh=ZDNlZDc0MzIxNw=="
            target="_blank">
            <FaInstagram size={26} />
          </a>
        </div>
        <div>
          <Link href={`/${language === 'en' ? 'pl' : 'en'}`}>
            <Image
              src={
                language === 'pl'
                  ? '/img/flag_poland.svg'
                  : '/img/flag_united_kingdom.svg'
              }
              alt="English Flag"
              width={35}
              height={30}
              className="inline-block"
            />
          </Link>
        </div>
      </div>
    </header>
  );
};

export default Header;
