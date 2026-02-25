# Snow Cafeteria Portal
[![Deploy Main](https://github.com/tjwalkr3/snow-cafeteria-portal/actions/workflows/deploy-main.yml/badge.svg)](https://github.com/tjwalkr3/snow-cafeteria-portal/actions/workflows/deploy-main.yml)
[![Merge Dev](https://github.com/tjwalkr3/snow-cafeteria-portal/actions/workflows/merge-dev.yml/badge.svg)](https://github.com/tjwalkr3/snow-cafeteria-portal/actions/workflows/merge-dev.yml)

## Project Structure
| Project | Type | Description |
|---------|------|-------------|
| **Cafeteria.Api** | Web API | Backend API service for the cafeteria system |
| **Cafeteria.Customer** | Blazor Server | Customer-facing web application for cafeteria services |
| **Cafeteria.Management** | Blazor Server | Management interface for cafeteria administration |
| **Cafeteria.Shared** | Shared Library | Contains DTOs and authorization logic and razorcomponents |
| **Cafeteria.UnitTests** | Test Project | All unit tests for the solution |
| **Cafeteria.IntegrationTests** | Test Project | All Integration tests for the solution |
| **ReceiptPrinter** | Python Project | The Pythin project that runs on the Raspberry pi to handle receipt printing |
| **Docs** | Documentation | Documents related to styling and ongoing development |
| **kube** | COnfigurations | YAML files for a Kubernetes deployment of our app |

## Running with Aspire
### Linux
```bash
./start-aspire.sh
```
