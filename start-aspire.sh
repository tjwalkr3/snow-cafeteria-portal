#!/bin/bash
# Script to cleanly stop Aspire and free up all ports

echo "Stopping Aspire and cleaning up..."

# Kill DCP processes specifically
pkill -9 -f "dcp" 2>/dev/null
pkill -9 -f "Aspire" 2>/dev/null
pkill -9 -f "dcpctrl" 2>/dev/null

# Force kill any processes on the Aspire ports
fuser -k 19000/tcp 19100/tcp 5132/tcp 5087/tcp 5248/tcp 2>/dev/null

# Wait a moment for ports to be released
sleep 2

# Clear Aspire DCP state
rm -rf ~/.aspire/dcpctrl 2>/dev/null

echo "All Aspire processes stopped."

dotnet run --project Cafeteria.AppHost/Cafeteria.AppHost.csproj