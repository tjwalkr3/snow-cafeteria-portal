#!/usr/bin/env pwsh
# Script to cleanly stop Aspire and free up all ports on Windows

Write-Host "Stopping Aspire and cleaning up..." -ForegroundColor Cyan

# Kill DCP processes specifically
Write-Host "Stopping DCP and Aspire processes..." -ForegroundColor Yellow
Get-Process -Name "*dcp*" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Get-Process -Name "*Aspire*" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Get-Process -Name "dcpctrl" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# Force kill any processes on the Aspire ports
$ports = @(19000, 19100, 5132, 5087, 5248)
foreach ($port in $ports) {
    $connections = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
    foreach ($conn in $connections) {
        $process = Get-Process -Id $conn.OwningProcess -ErrorAction SilentlyContinue
        if ($process) {
            Write-Host "Stopping process $($process.Name) (PID: $($process.Id)) using port $port" -ForegroundColor Yellow
            Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        }
    }
}

# Wait a moment for ports to be released
Write-Host "Waiting for ports to be released..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

# Clear Aspire DCP state
$aspireDir = Join-Path $env:USERPROFILE ".aspire\dcpctrl"
if (Test-Path $aspireDir) {
    Write-Host "Clearing Aspire DCP state..." -ForegroundColor Yellow
    Remove-Item -Path $aspireDir -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host "All Aspire processes stopped." -ForegroundColor Green
Write-Host ""
Write-Host "Starting Aspire AppHost..." -ForegroundColor Cyan

# Start the Aspire AppHost
dotnet watch --project Cafeteria.AppHost/Cafeteria.AppHost.csproj
