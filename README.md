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

## CI/CD Deployment Configuration (GitHub Actions)
To run the deploy workflow in [.github/workflows/deploy-main.yml](.github/workflows/deploy-main.yml), configure the following repository settings.

### GitHub Secrets (required)
| Secret | Purpose |
|---|---|
| `KUBE_CONFIG` | The kubernetes configuration. |
| `DOCKERHUB_TOKEN` | Token used to push application images to Docker Hub. |
| `ACME_EMAIL` | Email used by Let's Encrypt ACME registration in cert-manager. |
| `CLOUDFLARE_API_TOKEN` | Cloudflare API token used by cert-manager DNS-01 challenge. |
| `DEPLOY_MAIN_DISCORD` | Discord webhook URL used by the deployment notification step. |

### GitHub Variables (required)
| Variable | Purpose |
|---|---|
| `DOCKERHUB_USERNAME` | Docker Hub namespace used for image tags and login username. |
| `BASE_DOMAIN` | Base DNS zone used by ingress and wildcard cert resources (example: `dragonbytes.org`). |

### Self-hosted runner requirements (required for `deploy_to_aks`)
- The deploy job runs only on a self-hosted runner labeled `kube`.
- `kubectl` must already be installed and configured with access to the target cluster (`~/.kube/config`).
- If using Nix/Home Manager, ensure `$HOME/.nix-profile/bin` contains `kubectl` (the workflow exports this path via `GITHUB_PATH`).
- `envsubst` is installed at runtime by the workflow (`gettext-base`).
