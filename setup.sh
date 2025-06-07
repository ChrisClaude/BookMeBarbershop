#!/bin/bash
set -e

# Update package lists
sudo apt-get update

# Install .NET 9.0 SDK
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt-get update
sudo apt-get install -y dotnet-sdk-9.0

# Install Node.js 20.x (LTS)
curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
sudo apt-get install -y nodejs

# Verify installations
dotnet --version
node --version
npm --version

# Navigate to workspace
cd /mnt/persist/workspace

# Restore .NET dependencies
dotnet restore

# Build .NET solution
dotnet build --no-restore

# Navigate to frontend directory and install dependencies
cd bookme-ui
npm install

# Return to root directory
cd /mnt/persist/workspace

echo "Setup completed successfully!"