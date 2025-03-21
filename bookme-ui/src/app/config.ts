import { getConfig } from './utils/common.utils';
export const AZURE_AD_B2C_TENANT_NAME: string = getConfig(
  'AZURE_AD_B2C_TENANT_NAME',
  process.env.NEXT_PUBLIC_AZURE_AD_B2C_TENANT_NAME
);

export const AZURE_AD_B2C_CLIENT_ID: string = getConfig(
  'AZURE_AD_B2C_CLIENT_ID',
  process.env.NEXT_PUBLIC_AZURE_AD_B2C_CLIENT_ID
);

export const AZURE_AD_B2C_CLIENT_SECRET = getConfig(
  'AZURE_AD_B2C_CLIENT_SECRET',
  process.env.NEXT_PUBLIC_AZURE_AD_B2C_CLIENT_SECRET
);

export const AZURE_AD_B2C_PRIMARY_USER_FLOW: string = getConfig(
  'AZURE_AD_B2C_PRIMARY_USER_FLOW',
  process.env.NEXT_PUBLIC_AZURE_AD_B2C_PRIMARY_USER_FLOW
);

export const AZURE_AD_B2C_API_SCOPE: string = getConfig(
  'AZURE_AD_B2C_API_SCOPE',
  process.env.NEXT_PUBLIC_AZURE_AD_B2C_API_SCOPE
);

export const NEXT_PUBLIC_TENANT_DOMAIN: string = getConfig(
  'NEXT_PUBLIC_TENANT_DOMAIN',
  process.env.NEXT_PUBLIC_TENANT_DOMAIN
);

export const COOKIE_BOT_DOMAIN_GROUP_ID: string = getConfig(
  'COOKIE_BOT_DOMAIN_GROUP_ID',
  process.env.COOKIE_BOT_DOMAIN_GROUP_ID
);
