"use client";
import React, { useState } from "react";
import Logo from "./Logo";
import { CONTENT } from "@/_lib/utils/content.utils";
import useLanguageState from "@/_hooks/useLanguageState";
import Link from "next/link";
import "../../../node_modules/flag-icons/css/flag-icons.min.css";
import { FaInstagram } from "react-icons/fa6";
import { RiMenu3Line, RiCloseLine } from "react-icons/ri";
import Image from "next/image";
import { useAuth } from "@/_hooks/useAuth";
import { CgProfile } from "react-icons/cg";
import { Tooltip } from "@heroui/react";

const Header = () => {
  const { language } = useLanguageState();
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const { status, userProfile } = useAuth();

  const toggleMenu = () => setIsMenuOpen(!isMenuOpen);

  const NavLinks = () => (
    <ul className="flex flex-col lg:flex-row gap-6 uppercase">
      <li>
        <Link
          href="#services"
          className="nav-link"
          onClick={() => setIsMenuOpen(false)}
        >
          {CONTENT[language].home.services}
        </Link>
      </li>
      <li>
        <Link
          href="#gallery"
          className="nav-link"
          onClick={() => setIsMenuOpen(false)}
        >
          {CONTENT[language].home.gallery}
        </Link>
      </li>
      <li>
        <Link
          href="#contact"
          className="nav-link"
          onClick={() => setIsMenuOpen(false)}
        >
          {CONTENT[language].home.contact}
        </Link>
      </li>
    </ul>
  );

  const SocialAndLang = () => (
    <div className="flex items-center gap-x-6 lg:gap-x-36">
      <div className="flex gap-x-5 items-center">
        <a
          href="https://www.instagram.com/sans_tache_papi?utm_source=ig_web_button_share_sheet&igsh=ZDNlZDc0MzIxNw=="
          target="_blank"
        >
          <FaInstagram size={26} />
        </a>
      </div>
      <div>
        <Link href={`/${language === "en" ? "pl" : "en"}`}>
          <Image
            src={
              language === "pl"
                ? "/img/flag_poland.svg"
                : "/img/flag_united_kingdom.svg"
            }
            alt="English Flag"
            width={35}
            height={30}
            className="inline-block"
          />
        </Link>
      </div>
      {status === "authenticated" && (
        <div>
          <Tooltip content={`Navigate to ${userProfile?.userType} dashboard`}>
            <Link
              href={
                userProfile?.userType === "Admin"
                  ? "/admin"
                  : "/customer/bookings"
              }
            >
              <CgProfile size={30} className="text-primary" />
            </Link>
          </Tooltip>
        </div>
      )}
    </div>
  );

  return (
    <header>
      {/* Desktop Header */}
      <div className="hidden lg:flex justify-between items-center py-6 px-48">
        <div>
          <Link href="/">
            <Logo />
          </Link>
        </div>
        <nav>
          <NavLinks />
        </nav>
        <SocialAndLang />
      </div>

      {/* Mobile Header */}
      <div className="lg:hidden">
        <div className="flex justify-between items-center py-4 px-4">
          <Link href="/">
            <Logo />
          </Link>
          <button
            onClick={toggleMenu}
            className="text-2xl p-2"
            aria-label="Toggle Menu"
          >
            {isMenuOpen ? <RiCloseLine size={30} /> : <RiMenu3Line size={30} />}
          </button>
        </div>

        {/* Mobile Menu */}
        <div
          className={`${
            isMenuOpen ? "max-h-screen" : "max-h-0"
          } overflow-hidden transition-all duration-300 ease-in-out`}
        >
          <div className="px-4 py-6 flex flex-col gap-8 border-t border-gray-200">
            <nav>
              <NavLinks />
            </nav>
            <SocialAndLang />
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
