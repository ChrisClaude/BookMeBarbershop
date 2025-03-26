import React from 'react';
import Image from 'next/image';
import { Card, CardBody } from '@heroui/react';

const GALLERY_IMAGES = [
  {
    src: '/img/barber_gallery_1.jpeg',
    alt: 'Classic men haircut',
    title: 'Classic Cut'
  },
  {
    src: '/img/barber_gallery_2.jpeg',
    alt: 'Modern fade haircut',
    title: 'Modern Fade'
  },
  {
    src: '/img/barber_gallery_3.jpeg',
    alt: 'Beard trimming',
    title: 'Beard Grooming'
  },
  {
    src: '/img/barber_gallery_4.jpeg',
    alt: 'Hair styling',
    title: 'Styling'
  },
  {
    src: '/img/barber_gallery_5.jpeg',
    alt: 'Kids haircut',
    title: 'Kids Cut'
  },
  {
    src: '/img/barber_gallery_6.jpeg',
    alt: 'Fade',
    title: 'Fade'
  },
  {
    src: '/img/barber_gallery_7.jpeg',
    alt: 'Fade',
    title: 'Fade'
  }
];

const Gallery = () => {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {GALLERY_IMAGES.map((image, index) => (
        <Card
          key={index}
          className="group cursor-pointer overflow-hidden transition-transform duration-300 hover:scale-105"
          isPressable
        >
          <CardBody className="p-0 relative aspect-square overflow-hidden">
            <Image
              src={image.src}
              alt={image.alt}
              fill
              className="object-cover transition-transform duration-500 group-hover:scale-110"
              sizes="(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 33vw"
            />
            <div className="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-40 transition-all duration-300 flex items-end">
              <h3 className="text-white text-xl font-semibold p-4 translate-y-full group-hover:translate-y-0 transition-transform duration-300">
                {image.title}
              </h3>
            </div>
          </CardBody>
        </Card>
      ))}
    </div>
  );
};

export default Gallery;
