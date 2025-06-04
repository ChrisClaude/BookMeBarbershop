"use client";
import React from "react";

const SidebarNavigation = ({
  isMenuOpen,
  setIsMenuOpen,
}: {
  isMenuOpen: boolean;
  setIsMenuOpen: (value: boolean) => void;
}) => {
  if (!isMenuOpen) {
    return <></>;
  }

  return (
    <div
      className="hidden md:flex justify-end h-screen w-screen bg-transparent fixed top-0 left-0 z-50 p-1 rounded-md shadow-md"
      onClick={() => setIsMenuOpen(false)}
    >
      <div className="w-80 h-full bg-background text-foreground">
        <nav>
          <ul>
            <li>Bookings</li>
            <li>Profile</li>
            <li>Logout</li>
          </ul>
        </nav>
      </div>
    </div>
  );
};

export default SidebarNavigation;
