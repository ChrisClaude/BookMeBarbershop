"use client";
import Link from "next/link";
import React from "react";
import { CONTENT } from "@/_lib/utils/content.utils";
import useLanguageState from "@/_hooks/useLanguageState";

const NavLinks = ({ setIsMenuOpen }: { setIsMenuOpen: React.Dispatch<React.SetStateAction<boolean>> }) => {
  const { language } = useLanguageState();
  return (
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
); };

export default NavLinks;
