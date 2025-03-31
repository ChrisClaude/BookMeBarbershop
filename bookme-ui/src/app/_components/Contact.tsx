import React from 'react';
import { Card, CardBody, Divider } from '@heroui/react';
import {
  FaPhone,
  FaEnvelope,
  FaLocationDot,
  FaInstagram,
} from 'react-icons/fa6';
import { CONTENT } from '@/_lib/utils/content.utils';
import useLanguageState from '@/_hooks/useLanguageState';

const ContactInfo = ({
  icon,
  title,
  content,
  link,
}: {
  icon: React.ReactNode;
  title: string;
  content: string;
  link?: string;
}) => (
  <div className="flex items-center gap-5 p-4 rounded-lg bg-gray-100 hover:bg-gray-200 transition">
    <div className="text-primary p-4 bg-primary/10 rounded-full">{icon}</div>
    <div className="overflow-hidden">
      <h3 className="text-sm text-gray-600 font-semibold">{title}</h3>
      {link ? (
        <a
          href={link}
          className="text-lg font-medium text-gray-800 hover:text-primary transition-colors text-wrap"
          target="_blank"
          rel="noopener noreferrer">
          {content}
        </a>
      ) : (
        <p className="text-lg font-medium text-gray-800">{content}</p>
      )}
    </div>
  </div>
);

const Contact = () => {
  const { language } = useLanguageState();

  return (
    <div className="grid lg:grid-cols-3 gap-4 md:gap-6 lg:gap-8">
      {/* Contact Information */}
      <Card className="p-4 md:p-6 lg:p-8 shadow-lg col-span-full lg:col-span-2 order-2 lg:order-1">
        <CardBody>
          <h2 className="text-2xl md:text-3xl font-bold text-gray-900 mb-4 md:mb-6">
            {CONTENT[language].home.contactSection.getInTouch}
          </h2>

          <div className="grid sm:grid-cols-2 gap-4 md:gap-6">
            <ContactInfo
              icon={<FaPhone size={22} />}
              title={CONTENT[language].home.contactSection.phone}
              content="+48 729 553 607"
              link="tel:+48729553607"
            />
            <ContactInfo
              icon={<FaEnvelope size={22} />}
              title={CONTENT[language].home.contactSection.email}
              content="christ.tchambila@gmail.com"
              link="mailto:christ.tchambila@gmail.com"
            />
            <ContactInfo
              icon={<FaLocationDot size={22} />}
              title={CONTENT[language].home.contactSection.address}
              content="ul. Gasiorowkich 4D, 60-704 Poznań"
              link="https://maps.google.com/?q=ul.+Gasiorowkich+4D,+60-704+Poznań"
            />
          </div>

          <Divider className="my-4 md:my-6" />

          {/* Opening Hours */}
          <div>
            <h2 className="text-xl md:text-2xl font-semibold text-gray-900 mb-3 md:mb-4">
              {CONTENT[language].home.contactSection.openingHours}
            </h2>
            <div className="space-y-2 md:space-y-3 text-base md:text-lg text-gray-700">
              <div className="flex flex-col sm:flex-row sm:justify-between px-3 md:px-4 py-2 bg-gray-50 rounded-lg">
                <span>{CONTENT[language].home.contactSection.weekdays}</span>
                <span className="font-semibold">9:00 - 19:00</span>
              </div>
              <div className="flex flex-col sm:flex-row sm:justify-between px-3 md:px-4 py-2 bg-gray-50 rounded-lg">
                <span>{CONTENT[language].home.contactSection.saturday}</span>
                <span className="font-semibold">10:00 - 16:00</span>
              </div>
              <div className="flex flex-col sm:flex-row sm:justify-between px-3 md:px-4 py-2 bg-gray-50 rounded-lg">
                <span>{CONTENT[language].home.contactSection.sunday}</span>
                <span className="font-semibold">
                  {CONTENT[language].home.contactSection.closed}
                </span>
              </div>
            </div>
          </div>

          <Divider className="my-4 md:my-6" />

          {/* Social Media */}
          <div>
            <h2 className="text-xl md:text-2xl font-semibold text-gray-900 mb-3 md:mb-4">
              {CONTENT[language].home.contactSection.followUs}
            </h2>
            <div className="flex gap-4">
              <a
                href="https://www.instagram.com/sans_tache_papi?utm_source=ig_web_button_share_sheet&igsh=ZDNlZDc0MzIxNw=="
                target="_blank"
                rel="noopener noreferrer"
                aria-label="Instagram"
                className="p-3 bg-gradient-to-r from-pink-500 to-yellow-500 text-white rounded-full hover:opacity-80 transition-opacity active:scale-95">
                <FaInstagram size={20} />
              </a>
            </div>
          </div>
        </CardBody>
      </Card>

      {/* Map Section */}
      <Card className="h-[300px] md:h-[400px] lg:h-full shadow-lg col-span-full lg:col-span-1 order-1 lg:order-2">
        <CardBody className="p-0 h-full">
          <iframe
            src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d2434.3989895914942!2d16.90262025796878!3d52.39944470013142!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x47045b2d0bc95c39%3A0x76bdd654fd6e8667!2sG%C4%85siorowskich%204D%2C%2060-704%20Pozna%C5%84!5e0!3m2!1sen!2spl!4v1743094366659!5m2!1sen!2spl"
            className="w-full h-full border-0 rounded-lg"
            loading="lazy"
            referrerPolicy="no-referrer-when-downgrade"
            title="Barbershop location"
          />
        </CardBody>
      </Card>
    </div>
  );
};

export default Contact;
