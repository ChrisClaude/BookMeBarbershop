"use client";
import React from "react";
import Footer from "../Footer";
import Link from "next/link";
import Logo from "../Logo";
import { usePathname } from "next/navigation";

const Sidebar = () => {
  const pathName = usePathname();

  const isActive = (href: string) => {
    return pathName === href;
  };

  return (
    <aside className="h-full w-48 bg-gray-300 flex flex-col">
      <div className="bg-gray-300 p-4">
        <Link href="/admin">
          <Logo />
        </Link>
      </div>
      <nav className="flex flex-col">
        <Link
          href="/admin"
          className={`block p-2 hover:bg-gray-200 ${
            isActive("/admin") ? "bg-gray-200" : ""
          }`}
        >
          Dashboard
        </Link>
        <Link
          href="/admin/users"
          className={`block p-2 hover:bg-gray-200 ${
            isActive("/admin/users") ? "bg-gray-200" : ""
          }`}
        >
          Users
        </Link>
        <Link
          href="/admin/bookings"
          className={`block p-2 hover:bg-gray-200 ${
            isActive("/admin/bookings") ? "bg-gray-200" : ""
          }`}
        >
          Bookings
        </Link>
      </nav>
      <Footer className="py-4 mt-auto text-foreground bg-gray-300" />
    </aside>
  );
};

export default Sidebar;
