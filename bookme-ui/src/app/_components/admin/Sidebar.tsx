"use client";
import React from "react";
import Footer from "../Footer";
import Link from "next/link";
import Logo from "../Logo";
import { usePathname } from "next/navigation";
import { Button } from "@heroui/react";
import { CgProfile } from "react-icons/cg";
import { useAuth } from "@/_hooks/useAuth";
import { localLinks } from "@/_lib/enums/constant";

const Sidebar = () => {
  const pathName = usePathname();

  const isActive = (href: string) => {
    return pathName === href;
  };

  const { logout, userProfile } = useAuth();

  return (
    <aside className="h-full w-48 bg-gray-300 flex flex-col">
      <div className="p-4 flex flex-col gap-4">
        <Link href={localLinks.admin.dashboard}>
          <Logo />
        </Link>
        <div className="flex flex-col gap-2 items-center">
          <CgProfile size={60} className="text-primary" />
          <div className="flex flex-col gap-1">
            <p className="text-sm text-center">Admin</p>
            <p className="text-xs text-gray-500 text-center">
              {userProfile?.email}
            </p>
          </div>
          <Button
            onPress={() => logout()}
            className="hover:bg-red-600 hover:text-white"
          >
            Log out
          </Button>
        </div>
      </div>
      <nav className="flex flex-col">
        <Link
          href={localLinks.admin.dashboard}
          className={`block p-2 hover:bg-gray-200 ${
            isActive(localLinks.admin.dashboard) ? "bg-gray-200" : ""
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
