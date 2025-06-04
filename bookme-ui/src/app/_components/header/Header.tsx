"use client";
import React, { useState } from "react";
import Logo from "../Logo";
import Link from "next/link";
import { RiMenu3Line, RiCloseLine } from "react-icons/ri";
import SidebarNavigation from "../customer/SidebarNavigation";
import NavLinksExtension from "./NavLinksExtension";
import NavLinks from "./NavLinks";

const Header = () => {
  const [isMobileMenuOpen, setIsMenuOpen] = useState(false);
  const [isSidebarOpen, setIsSidebarOpen] = useState(false);

  const toggleMenu = () => setIsMenuOpen(!isMobileMenuOpen);

  return (
    <>
      <header>
        {/* Desktop Header */}
        <div className="hidden lg:flex justify-between items-center py-6 px-48">
          <div>
            <Link href="/">
              <Logo />
            </Link>
          </div>
          <nav>
            <NavLinks setIsMenuOpen={setIsMenuOpen} />
          </nav>
          <NavLinksExtension
            handleOpenSidebarNav={() => setIsSidebarOpen(true)}
          />
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
              {isMobileMenuOpen ? (
                <RiCloseLine size={30} />
              ) : (
                <RiMenu3Line size={30} />
              )}
            </button>
          </div>

          {/* Mobile Menu */}
          <div
            className={`${
              isMobileMenuOpen ? "max-h-screen" : "max-h-0"
            } overflow-hidden transition-all duration-300 ease-in-out`}
          >
            <div className="px-4 py-6 flex flex-col gap-8 border-t border-gray-200">
              <nav>
                <NavLinks setIsMenuOpen={setIsMenuOpen} />
              </nav>
              <NavLinksExtension
                handleOpenSidebarNav={() => setIsSidebarOpen(true)}
              />
            </div>
          </div>
        </div>
      </header>
      <SidebarNavigation
        isMenuOpen={isSidebarOpen}
        setIsMenuOpen={(value) => setIsSidebarOpen(value)}
      />
    </>
  );
};

export default Header;
