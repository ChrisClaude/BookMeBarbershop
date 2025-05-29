import { Button, DateValue, Modal, ModalBody, ModalContent, ModalFooter, ModalHeader } from "@heroui/react";
import React from "react";
import CreateTimeSlotForm from "./CreateTimeSlotForm";

const CreateTimeSlotFormModal = ({ selectedDate, isOpen, onClose }: { selectedDate: DateValue; isOpen: boolean; onClose: () => void }) => {
  return (
    <Modal isOpen={isOpen} size="lg" onClose={onClose}>
      <ModalContent>
        {(onClose) => (
          <>
            <ModalHeader className="flex flex-col gap-1">
              Create Time Slot
            </ModalHeader>
            <ModalBody>
              <CreateTimeSlotForm selectedDate={selectedDate} onSuccess={onClose} />
            </ModalBody>
            <ModalFooter>
              <Button color="danger" variant="light" onPress={onClose}>
                Close
              </Button>
            </ModalFooter>
          </>
        )}
      </ModalContent>
    </Modal>
  );
};

export default CreateTimeSlotFormModal;
