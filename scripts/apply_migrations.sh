#!/bin/bash

# Remove any quotes from the environment variables
CONNECTION_STRING=$(echo $CONNECTION_STRING | tr -d '"')

dotnet tool install --global dotnet-ef
dotnet ef database update --context BookMeContext --project BookMe.Infrastructure --startup-project BookMeAPI --connection "$CONNECTION_STRING"

echo "Migrations applied successfully."