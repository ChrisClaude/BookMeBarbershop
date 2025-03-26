'use client';
import useLanguageState from '@/_hooks/useLanguageState';
import { CONTENT } from '@/_lib/utils/content.utils';
import { Button } from '@heroui/react';

export default function Home() {
  const { language } = useLanguageState();

  return (
    <>
      <main className="py-28 px-48 banner-bg flex flex-col gap-y-16 h-[50rem]">
        <div className="flex flex-col gap-y-8 lg:w-[44rem] w-full">
          <h1 className="text-6xl font-bold uppercase">
            {CONTENT[language].home.bannerHeader}
          </h1>
          <p className="text-xl lg:w-w-[35rem] w-full">
            {CONTENT[language].home.bannerDescription}
          </p>
        </div>
        <div>
          <Button
            color="primary"
            size="lg"
            className="px-12 py-8 text-lg uppercase">
            {CONTENT[language].home.bookingActionButton}
          </Button>
        </div>
      </main>
      <button className="action-button">
        {CONTENT[language].home.bookingActionButton}
      </button>
    </>
  );
}
