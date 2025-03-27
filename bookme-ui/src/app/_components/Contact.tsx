import React from 'react';
import { Card, CardBody, Divider } from '@heroui/react';
import { FaPhone, FaEnvelope, FaLocationDot, FaFacebookF, FaInstagram } from 'react-icons/fa6';
import { CONTENT } from '@/_lib/utils/content.utils';
import useLanguageState from '@/_hooks/useLanguageState';

const ContactInfo = ({ icon, title, content, link }: {
  icon: React.ReactNode,
  title: string,
  content: string,
  link?: string
}) => (
  <div className="flex items-center gap-4 group">
    <div className="text-primary p-3 bg-primary/10 rounded-full">
      {icon}
    </div>
    <div>
      <h3 className="text-sm text-gray-500 font-medium">{title}</h3>
      {link ? (
        <a
          href={link}
          className="text-lg hover:text-primary transition-colors"
          target="_blank"
          rel="noopener noreferrer"
        >
          {content}
        </a>
      ) : (
        <p className="text-lg">{content}</p>
      )}
    </div>
  </div>
);

const Contact = () => {
  const { language } = useLanguageState();

  return (
    <div className="grid md:grid-cols-2 gap-8">
      {/* Contact Information */}
      <Card className="p-6">
        <CardBody>
          <h2 className="text-2xl font-semibold mb-6">
            {CONTENT[language].home.contactSection.getInTouch}
          </h2>

          <div className="space-y-6">
            <ContactInfo
              icon={<FaPhone size={20} />}
              title={CONTENT[language].home.contactSection.phone}
              content="+48 123 456 789"
              link="tel:+48123456789"
            />

            <ContactInfo
              icon={<FaEnvelope size={20} />}
              title={CONTENT[language].home.contactSection.email}
              content="robert@barbershop.pl"
              link="mailto:robert@barbershop.pl"
            />

            <ContactInfo
              icon={<FaLocationDot size={20} />}
              title={CONTENT[language].home.contactSection.address}
              content="ul. Półwiejska 42, 61-888 Poznań"
              link="https://maps.google.com/?q=ul.+Półwiejska+42,+61-888+Poznań"
            />
          </div>

          <Divider className="my-6" />

          {/* Opening Hours */}
          <div>
            <h2 className="text-2xl font-semibold mb-4">
              {CONTENT[language].home.contactSection.openingHours}
            </h2>
            <div className="space-y-2">
              <div className="flex justify-between">
                <span>{CONTENT[language].home.contactSection.weekdays}</span>
                <span>9:00 - 19:00</span>
              </div>
              <div className="flex justify-between">
                <span>{CONTENT[language].home.contactSection.saturday}</span>
                <span>10:00 - 16:00</span>
              </div>
              <div className="flex justify-between">
                <span>{CONTENT[language].home.contactSection.sunday}</span>
                <span>{CONTENT[language].home.contactSection.closed}</span>
              </div>
            </div>
          </div>

          <Divider className="my-6" />

          {/* Social Media */}
          <div>
            <h2 className="text-2xl font-semibold mb-4">
              {CONTENT[language].home.contactSection.followUs}
            </h2>
            <div className="flex gap-4">
              <a
                href="https://facebook.com/robertbarbershop"
                target="_blank"
                rel="noopener noreferrer"
                className="p-3 bg-primary/10 rounded-full hover:bg-primary hover:text-white transition-colors"
              >
                <FaFacebookF size={20} />
              </a>
              <a
                href="https://instagram.com/robertbarbershop"
                target="_blank"
                rel="noopener noreferrer"
                className="p-3 bg-primary/10 rounded-full hover:bg-primary hover:text-white transition-colors"
              >
                <FaInstagram size={20} />
              </a>
            </div>
          </div>
        </CardBody>
      </Card>

      {/* Map */}
      <Card className="h-full min-h-[400px]">
        <CardBody className="p-0 h-full">
          <iframe
            src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d2434.0370003216447!2d16.92451!3d52.40341!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x47045b3437!2zUG_Fgnx3aWVqc2thIDQyLCA2MS04ODggUG96bmHFhA!5e0!3m2!1sen!2spl!4v1635959123456!5m2!1sen!2spl"
            className="w-full h-full border-0"
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
