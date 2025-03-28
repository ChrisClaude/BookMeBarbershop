import React from 'react';
import Header from '@/_components/Header';
import Footer from '@/_components/Footer';

const DefaultLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <>
      <Header />
      <main>{children}</main>
      <Footer />
    </>
  );
};

export default DefaultLayout;
