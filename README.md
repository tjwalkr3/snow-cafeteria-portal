# Snow Cafeteria Portal
[![Lint, Check for Warnings, and Run the Tests](https://github.com/tjwalkr3/snow-cafeteria-portal/actions/workflows/code-checks.yml/badge.svg)](https://github.com/tjwalkr3/snow-cafeteria-portal/actions/workflows/code-checks.yml)

## Project Structure

| Project | Type | Description |
|---------|------|-------------|
| **Cafeteria.Api** | Web API | Backend API service for the cafeteria system |
| **Cafeteria.Api.Tests** | Test Project | Unit tests for the API project |
| **Cafeteria.Customer** | Blazor Server | Customer-facing web application for cafeteria services |
| **Cafeteria.Customer.Tests** | Test Project | Unit tests for the customer portal |
| **Cafeteria.Management** | Blazor Server | Management interface for cafeteria administration |
| **Cafeteria.Management.Tests** | Test Project | Unit tests for the management portal |


## Running with Aspire
### Option 1: Using dotnet run (Recommended)
```bash
./start-aspire.sh
```