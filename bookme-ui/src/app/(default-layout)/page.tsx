'use client';
import useLanguageState from '@/_hooks/useLanguageState';
import { CONTENT } from '@/_lib/utils/content.utils';
import { Button } from '@heroui/react';

export default function Home() {
  const { language } = useLanguageState();

  return (
    <div>
      <main className="py-28 px-48 banner-bg flex flex-col gap-y-16">
        <div className="flex flex-col gap-y-7 w-2/3">
          <h1 className="text-5xl font-bold uppercase">
            {CONTENT[language].home.bannerHeader}
          </h1>
          <p className="text-lg">{CONTENT[language].home.bannerDescription}</p>
        </div>
        <div>
          <Button color="primary" size="lg">
            {CONTENT[language].home.bookingActionButton}
          </Button>
        </div>
      </main>
    </div>
  );
}
