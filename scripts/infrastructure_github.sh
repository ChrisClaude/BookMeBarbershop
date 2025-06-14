#!/bin/bash

# Settings
KEYVAULT_NAME="kv-bm-prod-westeurope"
SECRET_NAME="SqlConnectionString"
GITHUB_SECRET_NAME="AZURE_SQL_CONNECTION_STRING"
GITHUB_REPO="ChrisClaude/BookMeBarbershop"

# Get secret from Azure Key Vault
connection_string=$(az keyvault secret show \
  --vault-name "$KEYVAULT_NAME" \
  --name "$SECRET_NAME" \
  --query value -o tsv)

# Save it to GitHub secret using GitHub CLI
gh secret set "$GITHUB_SECRET_NAME" \
  --repo "$GITHUB_REPO" \
  --body "$connection_string"