'use client';
import HomePageContent from '@/_components/HomePageContent';
import Loader from '@/_components/Loader';
import useLanguageState from '@/_hooks/useLanguageState';
import { useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';

const HomeLang = ({ params }: { params: { lang: string } }) => {
  const router = useRouter();
  const { language, switchLanguage } = useLanguageState();
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (params.lang === 'en' || params.lang === 'pl') {
      switchLanguage(params.lang);
      return;
    }
    router.push('/');
  }, [params.lang, router, switchLanguage]);

  useEffect(() => {
    setIsLoading(false);
  }, [language]);

  if (isLoading) {
    return <Loader />;
  }

  return <HomePageContent language={language} />;
};

export default HomeLang;
