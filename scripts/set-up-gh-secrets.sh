
GITHUB_REPO="ChrisClaude/BookMeBarbershop"

SECRETS_FILE=".env.local"

while IFS='=' read -r key value; do
  if [[ ! -z "$key" && ! "$key" =~ ^# ]]; then
    echo "Setting secret: $key"
    gh secret set "$key" \
      --repo "$GITHUB_REPO" \
      --body "$value"
  fi
done < "$SECRETS_FILE"