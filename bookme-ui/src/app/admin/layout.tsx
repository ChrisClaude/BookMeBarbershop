import React from "react";
import Sidebar from "@/_components/admin/Sidebar";

const DefaultLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className="w-screen h-screen bg-slate-200 flex overflow-hidden">
      <Sidebar />
      <main className="flex flex-col flex-1">
        {children}
      </main>
    </div>
  );
};

export default DefaultLayout;
