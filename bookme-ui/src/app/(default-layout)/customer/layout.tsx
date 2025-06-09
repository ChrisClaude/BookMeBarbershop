import React from "react";
import Header from "@/_components/header/Header";

const DefaultLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <>
      <Header />
      {children}
    </>
  );
};

export default DefaultLayout;
