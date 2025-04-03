import {
  Button,
  Modal,
  ModalBody,
  ModalContent,
  ModalFooter,
  ModalHeader,
} from "@heroui/react";
import { FaWhatsapp } from "react-icons/fa";
import { CONTENT } from "@/_lib/utils/content.utils";
import { Language } from "@/_lib/features/language/language-slice";
import useLanguageState from "@/_hooks/useLanguageState";

interface BannerModalProps {
  isOpen: boolean;
  onOpenChange: () => void;
}

const BannerModal = ({ isOpen, onOpenChange }: BannerModalProps) => {
  const { language } = useLanguageState();
  const whatsappNumber = "+48729553607";
  const whatsappLink = `https://wa.me/${whatsappNumber}`;

  const handleWhatsAppClick = () => {
    window.open(whatsappLink, '_blank');
  };

  return (
    <Modal
      isOpen={isOpen}
      onOpenChange={onOpenChange}
      backdrop="blur"
      classNames={{
        base: "border-none",
        header: "border-b-[1px] border-neutral-200",
        body: "py-6",
        backdrop: "bg-black/50 backdrop-blur-sm",
        closeButton: "hover:bg-neutral-200/50 active:bg-neutral-200/80"
      }}
    >
      <ModalContent>
        {(onClose) => (
          <>
            <ModalHeader className="flex flex-col gap-1">
              {CONTENT[language].modal.bookingNotAvailable}
            </ModalHeader>
            <ModalBody>
              <div className="space-y-4">
                <p className="text-neutral-700">
                  {CONTENT[language].modal.bookingMessage}
                </p>
                <div className="flex items-center gap-2 p-4 bg-neutral-100 rounded-lg">
                  <FaWhatsapp className="text-[#25D366] text-xl" />
                  <span className="font-medium">{whatsappNumber}</span>
                </div>
                <p className="text-sm text-neutral-500">
                  {CONTENT[language].modal.responseTime}
                </p>
              </div>
            </ModalBody>
            <ModalFooter className="flex gap-2">
              <Button
                color="danger"
                variant="light"
                onPress={onClose}
                className="flex-1"
              >
                {CONTENT[language].modal.close}
              </Button>
              <Button
                color="success"
                className="flex-1 bg-[#25D366] hover:bg-[#20bd5a]"
                onPress={handleWhatsAppClick}
                startContent={<FaWhatsapp />}
              >
                {CONTENT[language].modal.openWhatsApp}
              </Button>
            </ModalFooter>
          </>
        )}
      </ModalContent>
    </Modal>
  );
};

export default BannerModal;
