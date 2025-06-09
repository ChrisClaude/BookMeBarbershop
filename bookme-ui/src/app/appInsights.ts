import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';

const reactPlugin = new ReactPlugin();
let appInsights: ApplicationInsights | undefined;

// Initialize function to be called after config is loaded
export const initializeAppInsights = (connectionString: string | undefined | null) => {
  if (
    connectionString !== undefined &&
    connectionString !== '' &&
    connectionString !== null
  ) {
    appInsights = new ApplicationInsights({
      config: {
        connectionString: connectionString,
        enableAutoRouteTracking: true,
        extensions: [reactPlugin],
        disablePageUnloadEvents: ["unload"]
      },
    });

    appInsights.loadAppInsights();
  }
};

export { reactPlugin, appInsights };
