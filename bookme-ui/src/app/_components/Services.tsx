import { Language } from '@/_lib/features/language/language-slice';
import { CONTENT } from '@/_lib/utils/content.utils';
import {
  Table,
  TableBody,
  TableCell,
  TableColumn,
  TableHeader,
  TableRow,
} from '@heroui/react';
import React from 'react';

const Services = ({ language }: { language: Language }) => {
  return (
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
  );
};

export default Services;
