# TrainingNutrition — Azure Deployment Plan

## Status

**Not started.** This document is a placeholder.

Deployment to Azure is the final step, after both the .NET backend and the React frontend are complete and working locally.

---

## Architecture

| Component | Service | Why |
|---|---|---|
| .NET API | Azure Container Apps | Modern, scales to zero (near-zero cost), uses the existing multi-stage Dockerfile directly |
| React frontend | Azure Static Web Apps | Free tier, native GitHub Actions integration, purpose-built for SPAs |
| PostgreSQL | Neon (serverless, external) | Free tier, standard PostgreSQL — API connects via connection string |
| Redis | Upstash (serverless, external) | Free tier, standard Redis — API connects via connection string |
| Secrets | Environment variables on Container Apps | Connection strings and JWT secret injected at deploy time |
| CI/CD | GitHub Actions | Existing CI workflow extended with a deploy step on merge to `main` |

Neon and Upstash are external services (not Azure-managed). This keeps cost at effectively zero while still allowing the CV to read: *"Deployed on Azure (Container Apps + Static Web Apps) with automated CI/CD via GitHub Actions."*

---

## Prerequisites (before starting this phase)

- [x] Multi-stage Dockerfile complete
- [x] GitHub Actions CI passing
- [ ] Public GitHub repo live
- [ ] Azure account created

---

## To be defined

Step-by-step instructions (Azure Container Registry setup, Container Apps deployment, Static Web Apps workflow, environment variable configuration) will be added here when deployment becomes the active phase.
