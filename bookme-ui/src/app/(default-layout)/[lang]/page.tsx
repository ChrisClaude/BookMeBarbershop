'use client';
import HomePageContent from '@/_components/HomePageContent';
import useLanguageState from '@/_hooks/useLanguageState';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

const HomeLang = ({ params }: { params: { lang: string } }) => {
  const router = useRouter();
  const { language, switchLanguage } = useLanguageState();

  useEffect(() => {
    if (params.lang === 'en' || params.lang === 'pl') {
      switchLanguage(params.lang);
      return;
    }
    router.push('/');
  }, [params.lang, router, switchLanguage]);

  return <HomePageContent language={language} />;
};

export default HomeLang;
