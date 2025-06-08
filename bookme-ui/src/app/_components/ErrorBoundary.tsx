"use client";

import React from "react";
import { AppInsightsErrorBoundary } from "@microsoft/applicationinsights-react-js";
import { reactPlugin } from "../appInsights";
import Error from "../error";

const ErrorBoundary = ({ children }: { children: React.ReactNode }) => {
  return (
    <AppInsightsErrorBoundary
      appInsights={reactPlugin}
      onError={() => (
        <Error
          error={{
            name: "Error",
            message: "An error occurred",
            stack: undefined,
            cause: undefined,
            digest: undefined,
          }}
          reset={() => {}}
        />
      )}
    >
      <>{children}</>
    </AppInsightsErrorBoundary>
  );
};

export default ErrorBoundary;
