import React from 'react';
import { Card, CardBody, Divider } from '@heroui/react';
import { FaPhone, FaEnvelope, FaLocationDot, FaFacebookF, FaInstagram } from 'react-icons/fa6';
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
    <div className="text-primary p-4 bg-primary/10 rounded-full">
      {icon}
    </div>
    <div>
      <h3 className="text-sm text-gray-600 font-semibold">{title}</h3>
      {link ? (
        <a
          href={link}
          className="text-lg font-medium text-gray-800 hover:text-primary transition-colors"
          target="_blank"
          rel="noopener noreferrer"
        >
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
    <div className="grid lg:grid-cols-3 gap-8">
      {/* Contact Information */}
      <Card className="p-8 shadow-lg col-span-2">
        <CardBody>
          <h2 className="text-3xl font-bold text-gray-900 mb-6">
            {CONTENT[language].home.contactSection.getInTouch}
          </h2>

          <div className="grid md:grid-cols-2 gap-6">
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
              link="https://maps.google.com/?q=ul.+Półwiejska+42,+61-888+Poznań"
            />
          </div>

          <Divider className="my-6" />

          {/* Opening Hours */}
          <div>
            <h2 className="text-2xl font-semibold text-gray-900 mb-4">
              {CONTENT[language].home.contactSection.openingHours}
            </h2>
            <div className="space-y-3 text-gray-700 text-lg">
              <div className="flex justify-between px-4 py-2 bg-gray-50 rounded-lg">
                <span>{CONTENT[language].home.contactSection.weekdays}</span>
                <span className="font-semibold">9:00 - 19:00</span>
              </div>
              <div className="flex justify-between px-4 py-2 bg-gray-50 rounded-lg">
                <span>{CONTENT[language].home.contactSection.saturday}</span>
                <span className="font-semibold">10:00 - 16:00</span>
              </div>
              <div className="flex justify-between px-4 py-2 bg-gray-50 rounded-lg">
                <span>{CONTENT[language].home.contactSection.sunday}</span>
                <span className="font-semibold">{CONTENT[language].home.contactSection.closed}</span>
              </div>
            </div>
          </div>

          <Divider className="my-6" />

          {/* Social Media */}
          <div>
            <h2 className="text-2xl font-semibold text-gray-900 mb-4">
              {CONTENT[language].home.contactSection.followUs}
            </h2>
            <div className="flex gap-4">
              <a
                href="https://facebook.com/robertbarbershop"
                target="_blank"
                rel="noopener noreferrer"
                aria-label="Facebook"
                className="p-3 bg-blue-600 text-white rounded-full hover:bg-blue-700 transition"
              >
                <FaFacebookF size={20} />
              </a>
              <a
                href="https://instagram.com/robertbarbershop"
                target="_blank"
                rel="noopener noreferrer"
                aria-label="Instagram"
                className="p-3 bg-gradient-to-r from-pink-500 to-yellow-500 text-white rounded-full hover:opacity-80 transition"
              >
                <FaInstagram size={20} />
              </a>
            </div>
          </div>
        </CardBody>
      </Card>

      {/* Map Section */}
      <Card className="h-full min-h-[400px] shadow-lg">
        <CardBody className="p-0 h-full">
          <iframe
            src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d2434.0370003216447!2d16.92451!3d52.40341!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x47045b3437!2zUG_Fgnx3aWVqc2thIDQyLCA2MS04ODggUG96bmHFhA!5e0!3m2!1sen!2spl!4v1635959123456!5m2!1sen!2spl"
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