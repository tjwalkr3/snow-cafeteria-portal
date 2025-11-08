#!/usr/bin/env pwsh

dotnet watch --project Cafeteria.AppHost/Cafeteria.AppHost.csproj 2>&1 | Select-String "Login to the dashboard"
