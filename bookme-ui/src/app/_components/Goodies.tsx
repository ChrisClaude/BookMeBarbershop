import React from 'react';
import { CONTENT } from '@/_lib/utils/content.utils';
import { IoIosWifi } from 'react-icons/io';
import { IoDiamondOutline } from 'react-icons/io5';
import { PiCoffee } from 'react-icons/pi';
import { Language } from '@/_lib/features/language/language-slice';

const Goodies = ({ language }: { language: Language }) => {
  return (
    <section className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 py-12 md:py-20 lg:py-28 px-4 md:px-8 lg:px-48 gap-12 md:gap-16 lg:gap-28 bg-slate-50">
      <div className="flex flex-col gap-y-4 items-center text-center">
        <IoDiamondOutline size={90} className="w-16 md:w-20 lg:w-[90px]" />
        <h2 className="text-lg md:text-xl font-semibold">
          {CONTENT[language].home.goodies[0].title}
        </h2>
        <p className="text-center">
          {CONTENT[language].home.goodies[0].description}
        </p>
      </div>
      <div className="flex flex-col gap-y-4 items-center text-center">
        <PiCoffee size={90} className="w-16 md:w-20 lg:w-[90px]" />
        <h2 className="text-lg md:text-xl font-semibold">
          {CONTENT[language].home.goodies[1].title}
        </h2>
        <p className="text-center">
          {CONTENT[language].home.goodies[1].description}
        </p>
      </div>
      <div className="flex flex-col gap-y-4 items-center text-center md:col-span-2 lg:col-span-1">
        <IoIosWifi size={90} className="w-16 md:w-20 lg:w-[90px]" />
        <h2 className="text-lg md:text-xl font-semibold">
          {CONTENT[language].home.goodies[2].title}
        </h2>
        <p className="text-center">
          {CONTENT[language].home.goodies[2].description}
        </p>
      </div>
    </section>
  );
};

export default Goodies;
