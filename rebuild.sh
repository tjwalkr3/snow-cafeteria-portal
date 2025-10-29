#!/bin/bash

echo "Cleaning bin and obj folders..."

# Find and delete all bin folders
find . -type d -name "bin" -exec rm -rf {} + 2>/dev/null

# Find and delete all obj folders
find . -type d -name "obj" -exec rm -rf {} + 2>/dev/null

echo "Bin and obj folders deleted."
echo "Restoring packages locally for IntelliSense..."

dotnet restore

echo "Running docker compose down and rebuild..."

# Run docker compose commands
docker compose down -v && docker compose up --build
