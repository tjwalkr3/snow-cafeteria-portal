#!/bin/bash

dotnet watch --project Cafeteria.AppHost/Cafeteria.AppHost.csproj 2>&1 | grep --line-buffered "Login to the dashboard"