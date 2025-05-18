import { useAuth } from "@/_hooks/useAuth";
import { BookingService } from "@/_lib/services/booking.service";
import {
  Button,
  Checkbox,
  Form,
  Input,
  Select,
  SelectItem,
} from "@heroui/react";
import React, { useCallback } from "react";

export type ValidationError = string | string[];
export type ValidationErrors = Record<string, ValidationError>;
export interface ValidationResult {
  isInvalid: boolean;
  validationErrors: string[];
  validationDetails: ValidityState;
}

const BookingForm = () => {
  const [errors, setErrors] = React.useState<ValidationErrors>({});
  const { session } = useAuth();
  // const [formData, setFormData] = React.useState<{
  //   name: string;
  //   email: string;
  //   phoneNumber: string;
  //   bookingDate: string;
  //   bookingTime: string;
  //   terms: boolean;
  // } | null>(null);

  const displayNameErrorMessage = useCallback(
    ({ validationDetails }: ValidationResult): React.ReactNode => {
      if (validationDetails.valueMissing) {
        return "This field is required";
      }

      return errors.name;
    },
    [errors]
  );

  const onSubmit = useCallback(
    (e: React.FormEvent<HTMLFormElement>) => {
      e.preventDefault();
      // const data = Object.fromEntries(new FormData(e.currentTarget));

      BookingService.createTimeSlot({
        request: {
          createTimeSlotsDto: {
            startDateTime: new Date(),
            endDateTime: new Date(),
          },
        },
        token: session?.accessToken as string,
      });
    },
    [session?.accessToken]
  );

  return (
    <>
      <h2 className="text-2xl font-bold mb-12 text-center">Create time slot</h2>
      <Form
        className="w-full justify-center items-center space-y-4"
        validationErrors={errors}
        onSubmit={onSubmit}
      >
        <div className="flex flex-col gap-4 w-full lg:max-w-lg max-w-md">
          <Input
            isRequired
            errorMessage={displayNameErrorMessage}
            label="Name"
            labelPlacement="outside"
            name="name"
            placeholder="Enter your name"
          />

          <Input
            isRequired
            errorMessage={({ validationDetails }) => {
              if (validationDetails.valueMissing) {
                return "Please enter your email";
              }
              if (validationDetails.typeMismatch) {
                return "Please enter a valid email address";
              }
            }}
            label="Email"
            labelPlacement="outside"
            name="email"
            placeholder="Enter your email"
            type="email"
          />

          <Select
            isRequired
            label="Country"
            labelPlacement="outside"
            name="country"
            placeholder="Select country"
          >
            <SelectItem key="ar">Argentina</SelectItem>
            <SelectItem key="us">United States</SelectItem>
            <SelectItem key="ca">Canada</SelectItem>
            <SelectItem key="uk">United Kingdom</SelectItem>
            <SelectItem key="au">Australia</SelectItem>
          </Select>

          <Checkbox
            isRequired
            classNames={{
              label: "text-small",
            }}
            isInvalid={!!errors.terms}
            name="terms"
            validationBehavior="aria"
            value="true"
            onValueChange={() => setErrors((prev) => ({ ...prev, terms: "" }))}
          >
            I agree to the terms and conditions
          </Checkbox>

          {errors.terms && (
            <span className="text-danger text-small">{errors.terms}</span>
          )}

          <div className="flex gap-4">
            <Button className="w-full" color="primary" type="submit">
              Submit
            </Button>
          </div>
        </div>
      </Form>
    </>
  );
};

export default BookingForm;
