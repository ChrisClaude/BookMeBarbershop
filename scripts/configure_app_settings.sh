#!/bin/bash

# Remove any quotes from the environment variables
AZURE_AD_B2C_INSTANCE=$(echo $AZURE_AD_B2C_INSTANCE | tr -d '"')
AZURE_AD_B2C_DOMAIN=$(echo $AZURE_AD_B2C_DOMAIN | tr -d '"')
AZURE_AD_B2C_TENANT_ID=$(echo $AZURE_AD_B2C_TENANT_ID | tr -d '"')
AZURE_AD_B2C_CLIENT_ID=$(echo $AZURE_AD_B2C_CLIENT_ID | tr -d '"')
AZURE_AD_B2C_CLIENT_SECRET=$(echo $AZURE_AD_B2C_CLIENT_SECRET | tr -d '"')
TWILIO_ACCOUNT_SID=$(echo $TWILIO_ACCOUNT_SID | tr -d '"')
TWILIO_AUTH_TOKEN=$(echo $TWILIO_AUTH_TOKEN | tr -d '"')
TWILIO_VERIFY_SERVICE_SID=$(echo $TWILIO_VERIFY_SERVICE_SID | tr -d '"')

# Set app settings for Azure Web App
az webapp config appsettings set \
  --name "$AZURE_WEBAPP_NAME" \
  --resource-group "$AZURE_RESOURCE_GROUP" \
  --settings \
  "DEPLOY_VERSION=$BUILD_NUMBER" \
  "AppSettings__AllowedCorsOrigins__0=https://book-me-barbershop-sigma.vercel.app" \
  "AppSettings__AllowedCorsOrigins__1=https://sanstache.pl" \
  "AppSettings__AllowedCorsOrigins__2=https://www.sanstache.pl" \
  "AppSettings__CacheConfig__CacheType=memory" \
  "AppSettings__CacheConfig__Enabled=true" \
  "AppSettings__CacheConfig__CacheTime=10" \
  "AppSettings__EventConfig__Server=localhost:9092" \
  "AppSettings__EventConfig__Topic=bookme" \
  "AppSettings__EventConfig__Enabled=false" \
  "AppSettings__Serilog__MinimumLevel__Default=Information" \
  "AppSettings__Serilog__MinimumLevel__Override__Microsoft=Warning" \
  "AppSettings__Serilog__MinimumLevel__Override__System=Warning" \
  "AppSettings__Serilog__MinimumLevel__Override__Microsoft.AspNetCore=Warning" \
  "AppSettings__Serilog__MinimumLevel__Override__Microsoft.EntityFrameworkCore=Warning" \
  "AppSettings__Serilog__Properties__Application=BookMeAPI" \
  "AppSettings__Serilog__EnableFileLogging=false" \
  "AppSettings__OpenTelemetry__Seq__LogsUri=http://localhost:5341/ingest/otlp/v1/logs" \
  "AppSettings__OpenTelemetry__Seq__TracesUri=http://localhost:5341/ingest/otlp/v1/traces" \
  "AppSettings__OpenTelemetry__Seq__ApiKey=" \
  "IpRateLimiting__EnableEndpointRateLimiting=true" \
  "IpRateLimiting__StackBlockedRequests=false" \
  "IpRateLimiting__RealIpHeader=X-Real-IP" \
  "IpRateLimiting__ClientIdHeader=X-ClientId" \
  "IpRateLimiting__HttpStatusCode=429" \
  "IpRateLimiting__QuotaExceededMessage=You have exceeded the rate limit. Please try again later." \
  "IpRateLimiting__GeneralRules__0__Endpoint=*" \
  "IpRateLimiting__GeneralRules__0__Period=1s" \
  "IpRateLimiting__GeneralRules__0__Limit=3" \
  "IpRateLimiting__GeneralRules__1__Endpoint=*" \
  "IpRateLimiting__GeneralRules__1__Period=15m" \
  "IpRateLimiting__GeneralRules__1__Limit=100" \
  "IpRateLimiting__GeneralRules__2__Endpoint=POST:/api/phoneverification/send-code" \
  "IpRateLimiting__GeneralRules__2__Period=1m" \
  "IpRateLimiting__GeneralRules__2__Limit=5" \
  "IpRateLimiting__GeneralRules__3__Endpoint=POST:/api/phoneverification/verify-code" \
  "IpRateLimiting__GeneralRules__3__Period=1m" \
  "IpRateLimiting__GeneralRules__3__Limit=10" \
  "AppSettings__AzureAdB2C__Instance=$AZURE_AD_B2C_INSTANCE" \
  "AppSettings__AzureAdB2C__Domain=$AZURE_AD_B2C_DOMAIN" \
  "AppSettings__AzureAdB2C__TenantId=$AZURE_AD_B2C_TENANT_ID" \
  "AppSettings__AzureAdB2C__ClientId=$AZURE_AD_B2C_CLIENT_ID" \
  "AppSettings__AzureAdB2C__ClientSecret=$AZURE_AD_B2C_CLIENT_SECRET" \
  "AppSettings__AzureAdB2C__CallbackPath=/signin-oidc" \
  "AppSettings__AzureAdB2C__SignedOutCallbackPath=/signout-callback-oidc" \
  "AppSettings__AzureAdB2C__SignUpSignInPolicyId=B2C_1_signup_signin" \
  "AppSettings__TwilioConfig__MinSecondsBetweenRequests=60" \
  "AppSettings__TwilioConfig__AccountSid=$TWILIO_ACCOUNT_SID" \
  "AppSettings__TwilioConfig__AuthToken=$TWILIO_AUTH_TOKEN" \
  "AppSettings__TwilioConfig__VerifyServiceSid=$TWILIO_VERIFY_SERVICE_SID" \
  "AppSettings__ApplicationInsights__ConnectionString=" + $APPLICATION_INSIGHTS_CONNECTION_STRING
