import React from 'react';
import { CONTENT } from '@/_lib/utils/content.utils';
import {
  Button,
  Table,
  TableBody,
  TableCell,
  TableColumn,
  TableHeader,
  TableRow,
} from '@heroui/react';
import Image from 'next/image';
import { IoIosWifi } from 'react-icons/io';
import { IoDiamondOutline } from 'react-icons/io5';
import { PiCoffee } from 'react-icons/pi';
import { Language } from '@/_lib/features/language/language-slice';
import Gallery from './Gallery';
import Contact from './Contact';

const HomePageContent = ({ language }: { language: Language }) => {
  return (
    <>
      {/* Banner */}
      <section>
        <div className="relative py-12 md:py-20 lg:py-28 px-4 md:px-8 lg:px-48 banner-bg flex flex-col gap-y-8 md:gap-y-12 lg:gap-y-16 min-h-[40rem] lg:h-[52rem]">
          <div className="flex flex-col gap-y-4 md:gap-y-6 lg:gap-y-8 w-full lg:w-[44rem]">
            <h1 className="text-4xl md:text-5xl lg:text-6xl font-bold uppercase">
              {CONTENT[language].home.bannerHeader}
            </h1>
            <p className="text-lg md:text-xl w-full lg:w-[35rem]">
              {CONTENT[language].home.bannerDescription}
            </p>
          </div>
          <div>
            <Button
              color="primary"
              size="lg"
              className="px-8 py-6 md:px-10 md:py-7 lg:px-12 lg:py-8 text-base md:text-lg uppercase">
              {CONTENT[language].home.bookingActionButton}
            </Button>
          </div>

          <div className="relative lg:absolute lg:bottom-0 lg:right-24 mt-8 lg:mt-0">
            <Image
              src="/img/banner_image.png"
              width={600}
              height={600}
              alt="Picture of a barber"
              // className="w-full max-w-[300px] md:max-w-[400px] lg:w-[800px] mx-auto"
            />
          </div>
        </div>
        <button className="action-button writing-v-rl hidden lg:block">
          {CONTENT[language].home.bookNow}
        </button>
      </section>
      {/* Goodies */}
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
      {/* Services */}
      <section className="py-12 md:py-20 lg:py-28 px-4 md:px-8 lg:px-48">
        <h1
          className="text-2xl md:text-3xl font-bold uppercase text-center mb-6 md:mb-8"
          id="services">
          {CONTENT[language].home.serviceSection.title}
        </h1>
        <div className="overflow-x-auto">
          <Table aria-label="collection of services" isStriped>
            <TableHeader>
              <TableColumn className="text-base md:text-lg lg:text-xl whitespace-nowrap">
                {CONTENT[language].home.serviceSection.tableData.headers[0]}
              </TableColumn>
              <TableColumn className="text-base md:text-lg lg:text-xl whitespace-nowrap">
                {CONTENT[language].home.serviceSection.tableData.headers[1]}
              </TableColumn>
              <TableColumn className="text-base md:text-lg lg:text-xl whitespace-nowrap">
                {CONTENT[language].home.serviceSection.tableData.headers[2]}
              </TableColumn>
            </TableHeader>
            <TableBody>
              {CONTENT[language].home.serviceSection.tableData.rows.map(
                (row, index) => (
                  <TableRow key={index}>
                    <TableCell className="text-sm md:text-base lg:text-lg whitespace-nowrap">
                      {row.name}
                    </TableCell>
                    <TableCell className="text-sm md:text-base lg:text-lg whitespace-nowrap">
                      {row.price}
                    </TableCell>
                    <TableCell className="text-sm md:text-base lg:text-lg whitespace-nowrap">
                      {row.estimatedTime}
                    </TableCell>
                  </TableRow>
                )
              )}
            </TableBody>
          </Table>
        </div>
      </section>
      {/* Gallery */}
      <section className="py-12 md:py-20 lg:py-28 px-4 md:px-8 lg:px-48 bg-slate-50">
        <h1
          className="text-2xl md:text-3xl font-bold uppercase text-center mb-6 md:mb-8"
          id="gallery">
          {CONTENT[language].home.gallerySection.title}
        </h1>
        <Gallery />
      </section>
      {/* Contact */}
      <section className="py-12 md:py-20 lg:py-28 px-4 md:px-8 lg:px-48">
        <h1
          className="text-2xl md:text-3xl font-bold uppercase text-center mb-6 md:mb-8"
          id="contact">
          {CONTENT[language].home.contactSection.title}
        </h1>
        <Contact />
      </section>
    </>
  );
};

export default HomePageContent;
