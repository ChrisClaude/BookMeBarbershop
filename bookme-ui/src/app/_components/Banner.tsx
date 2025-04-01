import React, { useCallback } from 'react';
import { CONTENT } from '@/_lib/utils/content.utils';
import { Button } from '@heroui/react';
import Image from 'next/image';
import Ripple from './Ripple';
import { Language } from '@/_lib/features/language/language-slice';
import { useRouter } from 'next/navigation';

const Banner = ({ language }: { language: Language }) => {
  const router = useRouter();

  const navigateToCustomerPortal = useCallback(() => {
    router.push('/customer/bookings');
  }, [router]);

  return (
    <section className="relative bg-banner py-12 md:py-20 lg:py-28 px-4 md:px-8 lg:px-48 min-h-[40rem] h-[52rem]">
      <div className="py-20 lg:py-0 px-8 lg:px-0 absolute top-0 left-0 z-10 w-full h-full bg-[#d5d5d5] bg-opacity-75 lg:bg-transparent lg:relative flex flex-col gap-y-8 md:gap-y-12 lg:gap-y-16">
        <div className="flex flex-col gap-y-4 md:gap-y-6 lg:gap-y-8 w-5/6 lg:w-[44rem]">
          <h1 className="text-4xl/snug md:text-5xl lg:text-6xl font-bold uppercase">
            {CONTENT[language].home.bannerHeader}
          </h1>
          <p className="text-xl/relaxed w-full lg:w-[35rem]">
            {CONTENT[language].home.bannerDescription}
          </p>
        </div>
        <div>
          <Button
            color="primary"
            size="lg"
            className="px-10 py-7 lg:px-12 lg:py-8 text-base md:text-lg uppercase"
            onPress={navigateToCustomerPortal}>
            {CONTENT[language].home.bookingActionButton}
          </Button>
        </div>
      </div>

      <div className="absolute bottom-0 right-0 lg:right-24">
        <Image
          src="/img/banner_image.png"
          width={600}
          height={600}
          alt="Picture of a barber"
          className=""
        />
      </div>
      <button
        className="action-button writing-v-rl relative overflow-hidden"
        onClick={navigateToCustomerPortal}>
        <Ripple color="#ffffff" duration={850} />
        {CONTENT[language].home.bookNow}
      </button>
    </section>
  );
};

export default Banner;
