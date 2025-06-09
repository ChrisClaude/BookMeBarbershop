import React from 'react';
import { Language } from '@/_lib/features/language/language-slice';
import Gallery from './Gallery';
import Contact from './Contact';
import Header from './header/Header';
import Services from './Services';
import Goodies from './Goodies';
import Banner from './Banner';
import Box from './Box';

const HomePageContent = ({ language }: { language: Language }) => {
  return (
    <>
      <Box className="bg-banner">
        <Header />
        <Banner language={language} />
      </Box>
      <Goodies language={language} />
      <Services language={language} />
      <Gallery language={language} />
      <Contact language={language} />
    </>
  );
};

export default HomePageContent;
