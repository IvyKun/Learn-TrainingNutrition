# TrainingNutrition

[![CI](https://github.com/IvyKun/Learn-TrainingNutrition/actions/workflows/ci.yml/badge.svg)](https://github.com/IvyKun/Learn-TrainingNutrition/actions/workflows/ci.yml)

A nutrition tracking application built with .NET 10 and Clean Architecture. Users register, authenticate, and manage an ingredient library. The application is designed to support full daily nutrition logging — meals, macronutrients, body weight, and activity — currently in active development toward an Azure deployment.

---

## Backend Stack

| Technology | Role |
|---|---|
| .NET 10 · ASP.NET Core Minimal APIs | Web framework |
| Clean Architecture | Strict inward dependency rule across four layers |
| MediatR (CQRS) | Command/Query separation — one handler per operation |
| FluentValidation + Pipeline Behavior | Validation runs automatically on every command, outside handlers |
| ASP.NET Core Identity + JWT | User management, password hashing, stateless auth |
| EF Core + PostgreSQL | Data access via Fluent API configurations |
| Redis + HybridCache | Two-level cache (L1 in-process, L2 Redis) with stampede protection |
| Serilog | Structured logging + HTTP request logging |
| Docker · Docker Compose | Multi-stage image (sdk → alpine runtime); PostgreSQL + Redis containers |
| GitHub Actions CI | Automated pipeline with a real PostgreSQL service container |
| xUnit + Moq + WebApplicationFactory | Unit tests (handler isolation) and integration tests (full HTTP stack) |

## Frontend Stack

| Technology | Role |
|---|---|
| React 19 + TypeScript | UI framework |
| Vite | Build tool — Rolldown (Rust bundler), ES module dev server |
| React Router v7 | SPA navigation + JWT-based protected routes |
| TanStack Query | Server state — fetch, cache, invalidation via `useQuery` / `useMutation` |
| Axios | HTTP client with JWT interceptor |
| React Hook Form + Zod v4 | Form management + schema validation (mirrors backend FluentValidation rules) |
| Tailwind CSS v4 + shadcn/ui | Utility-first styling + accessible Radix-based components |

---

## Architecture

```
API  ──►  Application  ──►  Domain
Infrastructure  ──►  Application  ──►  Domain
```

Domain has zero external dependencies. Application defines interfaces; Infrastructure provides the implementations. The API layer sends MediatR messages — it never calls repositories or EF Core directly.

| Pattern | What it does here |
|---|---|
| **Repository Pattern** | `IIngredientRepository` in Application, `EfIngredientRepository` in Infrastructure. Handlers are tested in isolation with Moq. |
| **CQRS + MediatR** | Each operation is a `Command` or `Query` with a dedicated handler. New features add new classes — no existing handler is modified. |
| **Pipeline Behavior** | `ValidationBehavior` and `LoggingBehavior` intercept every MediatR request automatically. |
| **Value Objects** | `Email`, `Grams`, `Macronutrients` validate in their constructors. Invalid domain state cannot be constructed. |
| **GlobalExceptionHandler** | Converts all unhandled exceptions to Problem Details (RFC 7807). One place for all error formatting. |
| **HybridCache** | `GET /ingredients/{id}` cached at handler level. Cache invalidated on update and delete. |

For the reasoning behind every technology choice, see [ARCHITECTURE_DECISIONS.md](ARCHITECTURE_DECISIONS.md).

---

## API

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| `POST` | `/auth/register` | — | Register → `201` |
| `POST` | `/auth/login` | — | Login → JWT string |
| `POST` | `/ingredients` | Bearer | Create ingredient → `201` |
| `GET` | `/ingredients` | Bearer | List or search by name / brand |
| `GET` | `/ingredients/{id}` | Bearer | Get ingredient (cached) → `200` / `404` |
| `PUT` | `/ingredients/{id}` | Bearer | Update ingredient → `204` |
| `DELETE` | `/ingredients/{id}` | Bearer | Delete ingredient → `204` |
| `GET` | `/dailylogs/{date}` | Bearer | Get or create daily log → `200` |
| `GET` | `/health` | — | Health check (PostgreSQL + Redis) |

---

## Testing

**94 tests passing** across two strategies:

- **Unit tests** — every MediatR handler tested in isolation with Moq; no database required
- **Integration tests** — full HTTP stack via `WebApplicationFactory` against a real PostgreSQL database; same setup used in CI

---

## Status

Authentication, ingredient management (full CRUD with caching), and daily log creation are complete and tested. The next milestone is the full daily log flow: meals, dishes with ingredient entries, and health metrics (weight, sleep, steps). The project is being built toward a publicly deployable app on Azure.
