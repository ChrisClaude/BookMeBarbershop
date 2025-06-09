import React from "react";

const FormSuccessErrorDisplay = ({
  errors,
  success,
  successMessage,
}: {
  errors: { field: string; message: string }[];
  success: boolean;
  successMessage: string;
}) => {
  return (
    <>
      {errors.length > 0 && (
        <div className="p-4 border rounded-lg bg-red-50 text-center">
          {errors.map((error, index) => (
            <p className="text-red-500" key={index}>
              {error.field} - {error.message}
            </p>
          ))}
        </div>
      )}
      {success && (
        <div className="p-4 border rounded-lg bg-green-50 text-center mb-4">
          <p className="text-green-600">{successMessage}</p>
        </div>
      )}
    </>
  );
};

export default FormSuccessErrorDisplay;
