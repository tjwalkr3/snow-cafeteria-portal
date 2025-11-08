#!/bin/bash

echo "Starting Aspire AppHost with watch mode..."
dotnet watch --project Cafeteria.AppHost/Cafeteria.AppHost.csproj 2>&1 | grep --line-buffered "Login to the dashboard"
