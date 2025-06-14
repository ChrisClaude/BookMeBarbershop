#!/bin/bash

# Set app settings for Azure Web App
az webapp config appsettings set \
  --name "$AZURE_WEBAPP_NAME" \
  --resource-group "$AZURE_RESOURCE_GROUP" \
  --settings \
  "DEPLOY_VERSION=$BUILD_NUMBER" \
  "AppSettings__AllowedCorsOrigins__0=https://your-frontend-url.com" \
  "AppSettings__CacheConfig__CacheType=memory" \
  "AppSettings__CacheConfig__Enabled=true" \
  # Add more settings as needed