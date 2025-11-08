#!/usr/bin/env pwsh

Write-Host "Starting Aspire AppHost with watch mode..." -ForegroundColor Cyan
dotnet watch --project Cafeteria.AppHost/Cafeteria.AppHost.csproj 2>&1 | Select-String "Login to the dashboard"
