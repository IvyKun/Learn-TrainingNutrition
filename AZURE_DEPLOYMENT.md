# TrainingNutrition — Azure Deployment Plan

## Status

**Not started.** This document is a placeholder.

Deployment to Azure is the final step, after both the .NET backend and the React frontend are complete and working locally.

---

## Scope

This document will cover how to publish the full application to Azure:

- .NET API → Azure App Service
- React frontend → Azure Static Web Apps (or App Service)
- PostgreSQL → Azure Database for PostgreSQL
- Redis → Azure Cache for Redis
- Environment variables and secrets → Azure App Configuration / Key Vault
- CI/CD → GitHub Actions deploying to Azure automatically on merge to `master`

---

## Prerequisites (before starting this phase)

- [ ] Backend feature-complete (all ROADMAP endpoints done)
- [ ] Frontend feature-complete (all pages working against local API)
- [ ] Both running correctly with Docker locally
- [ ] GitHub Actions CI passing on every push

---

## To be defined

The specific services, pricing tiers, and step-by-step instructions will be added here when deployment becomes the active phase.
