import React from 'react';
import Footer from '@/_components/Footer';

const DefaultLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <>
      <main>{children}</main>
      <Footer />
    </>
  );
};

export default DefaultLayout;
