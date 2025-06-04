"use client";
import React, { useRef, useEffect } from "react";
import Link from "next/link";
import { useAuth } from "@/_hooks/useAuth";
import { RiCloseLine } from "react-icons/ri";
import { CgProfile } from "react-icons/cg";
import Logo from "../Logo";
import { Button } from "@heroui/react";

const SidebarNavigation = ({
  isMenuOpen,
  setIsMenuOpen,
}: {
  isMenuOpen: boolean;
  setIsMenuOpen: (value: boolean) => void;
}) => {
  const sidebarRef = useRef<HTMLDivElement>(null);
  const { logout, userProfile } = useAuth();

  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (sidebarRef.current && !sidebarRef.current.contains(event.target as Node)) {
        setIsMenuOpen(false);
      }
    };

    if (isMenuOpen) {
      document.addEventListener("mousedown", handleClickOutside);
    }

    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [isMenuOpen, setIsMenuOpen]);

  if (!isMenuOpen) {
    return null;
  }

  return (
    <div className="fixed inset-0 bg-black/30 backdrop-blur-sm z-50 transition-all duration-300">
      <div
        ref={sidebarRef}
        className="w-80 h-full bg-background text-foreground ml-auto shadow-xl animate-in slide-in-from-right duration-300"
      >
        <div className="p-4 flex justify-between items-center border-b">
          <Logo />
          <button
            onClick={() => setIsMenuOpen(false)}
            className="p-2 rounded-full hover:bg-gray-100"
            aria-label="Close sidebar"
          >
            <RiCloseLine size={24} />
          </button>
        </div>

        <div className="p-6 flex flex-col items-center border-b">
          <CgProfile size={60} className="text-primary mb-3" />
          <p className="font-medium">{userProfile?.email}</p>
        </div>

        <nav className="p-4">
          <ul className="space-y-2">
            <li>
              <Link
                href="/customer/bookings"
                className="block p-3 rounded-lg hover:bg-gray-100 transition-colors"
                onClick={() => setIsMenuOpen(false)}
              >
                Bookings
              </Link>
            </li>
            <li>
              <Link
                href="/customer/profile"
                className="block p-3 rounded-lg hover:bg-gray-100 transition-colors"
                onClick={() => setIsMenuOpen(false)}
              >
                Profile
              </Link>
            </li>
            <li className="pt-4 mt-4 border-t">
              <Button
                onPress={() => logout()}
                className="w-full justify-center hover:bg-red-600 hover:text-white"
              >
                Logout
              </Button>
            </li>
          </ul>
        </nav>
      </div>
    </div>
  );
};

export default SidebarNavigation;
