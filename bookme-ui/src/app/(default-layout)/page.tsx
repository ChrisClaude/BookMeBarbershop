'use client';
import useLanguageState from '@/_hooks/useLanguageState';
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

export default function Home() {
  const { language } = useLanguageState();

  return (
    <>
      {/* Banner */}
      <section>
        <div className="relative py-28 px-48 banner-bg flex flex-col gap-y-16 h-[52rem]">
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

          <div className="absolute bottom-0 right-24">
            <Image
              src="/img/banner_image.png"
              width={600}
              height={600}
              alt="Picture of a barber"
            />
          </div>
        </div>
        <button className="action-button writing-v-rl">
          {CONTENT[language].home.bookNow}
        </button>
      </section>
      {/* Goodies */}
      <section className="grid grid-cols-3 py-28 px-48 gap-28 bg-slate-50">
        <div className="flex flex-col gap-y-4 items-center">
          <IoDiamondOutline size={90} />
          <h2 className="text-xl font-semibold">
            {CONTENT[language].home.goodies[0].title}
          </h2>
          <p className="text-center">
            {CONTENT[language].home.goodies[0].description}
          </p>
        </div>
        <div className="flex flex-col gap-y-4 items-center">
          <PiCoffee size={90} />
          <h2 className="text-xl font-semibold">
            {CONTENT[language].home.goodies[1].title}
          </h2>
          <p className="text-center">
            {CONTENT[language].home.goodies[1].description}
          </p>
        </div>
        <div className="flex flex-col gap-y-4 items-center">
          <IoIosWifi size={90} />
          <h2 className="text-xl font-semibold">
            {CONTENT[language].home.goodies[2].title}
          </h2>
          <p className="text-center">
            {CONTENT[language].home.goodies[2].description}
          </p>
        </div>
      </section>
      {/* Services */}
      <section className="py-28 px-48">
        <h1 className="text-3xl font-bold uppercase text-center mb-8">
          Services
        </h1>
        <Table aria-label="collection of services" isStriped>
          <TableHeader>
            <TableColumn className="text-xl">
              {CONTENT[language].home.serviceSection.tableData.headers[0]}
            </TableColumn>
            <TableColumn className="text-xl">
              {CONTENT[language].home.serviceSection.tableData.headers[1]}
            </TableColumn>
            <TableColumn className="text-xl">
              {CONTENT[language].home.serviceSection.tableData.headers[2]}
            </TableColumn>
          </TableHeader>
          <TableBody>
            {CONTENT[language].home.serviceSection.tableData.rows.map(
              (row, index) => (
                <TableRow key={index}>
                  <TableCell className="text-lg">{row.name}</TableCell>
                  <TableCell className="text-lg">{row.price}</TableCell>
                  <TableCell className="text-lg">{row.estimatedTime}</TableCell>
                </TableRow>
              )
            )}
          </TableBody>
        </Table>
      </section>
      {/* Gallery */}
      <section className="py-28 px-48 bg-slate-50">
        <h1 className="text-3xl font-bold uppercase text-center mb-8">
          Gallery
        </h1>
      </section>
      {/* Contact */}
      <section className="py-28 px-48">
        <h1 className="text-3xl font-bold uppercase text-center mb-8">
          Contact
        </h1>
      </section>
    </>
  );
}
