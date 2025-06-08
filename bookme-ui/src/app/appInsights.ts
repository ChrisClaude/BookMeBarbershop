import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { ReactPlugin } from '@microsoft/applicationinsights-react-js';
import { APP_INSIGHTS_CONNECTION_STRING } from './config';

const reactPlugin = new ReactPlugin();
let appInsights: ApplicationInsights | undefined;

if (
  APP_INSIGHTS_CONNECTION_STRING !== undefined &&
  APP_INSIGHTS_CONNECTION_STRING !== ''
  && APP_INSIGHTS_CONNECTION_STRING !== null
) {
  appInsights = new ApplicationInsights({
    config: {
      connectionString: APP_INSIGHTS_CONNECTION_STRING, // Replace with your App Insights connection string
      enableAutoRouteTracking: true, // Automatically track page views  }
      extensions: [reactPlugin],
      disablePageUnloadEvents: ["unload"]
    },
  });

  appInsights.loadAppInsights();
}

export { reactPlugin, appInsights };
