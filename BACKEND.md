# BACKEND.md — TrainingNutrition .NET Backend

## Project Context

**App:** Nutrition tracking REST API
**Stack:** .NET 10, ASP.NET Core Minimal APIs, EF Core, PostgreSQL, MediatR, FluentValidation, ASP.NET Core Identity, JWT, Redis + HybridCache, xUnit + Moq, Docker
**Architecture:** Clean Architecture

**Dependency Rule (NEVER break this):**
```
API → Application → Domain
Infrastructure → Application → Domain
```
Domain knows nothing about anyone. Application knows nothing about Infrastructure or API.

---

## Solution Structure

```
TrainingNutrition/
├── TrainingNutrition.slnx
├── Dockerfile                            ✅ multi-stage build (sdk:10.0 → aspnet:10.0-alpine)
├── .dockerignore                         ✅ excludes bin/, obj/, .git/, Tests/
├── docker-compose.yml                    ✅ PostgreSQL 17 + Redis 7 + API container
├── .github/workflows/ci.yml             ✅ GitHub Actions CI
├── TrainingNutrition.Domain/
│   ├── Common/       (Email, Grams, Macronutrients)
│   ├── Dishes/       (Dish)
│   ├── Ingredients/  (Ingredient, IngredientEntry)
│   ├── Meals/        (Meal, MealType)
│   ├── Tracking/     (DailyLog)
│   └── Users/        (User)
├── TrainingNutrition.Infrastructure/
│   ├── AppDbContext.cs                   (IdentityDbContext<AppUser>)
│   ├── AppUser.cs                        (AppUser : IdentityUser)
│   ├── Identity/                         (IdentityService, JwtSettings)
│   ├── Migrations/                       (InitialCreate ✅, AddIdentity ✅)
│   ├── Repositories/                     (EfIngredientRepository, EfDailyLogRepository)
│   └── Configurations/                   (Fluent API for all 6 entities)
├── TrainingNutrition.Application/
│   ├── Abstractions/                     (IIngredientRepository, IDailyLogRepository, IIdentityService)
│   ├── Behaviors/                        (ValidationBehavior, LoggingBehavior)
│   ├── Ingredients/                      (Create command, GetById query, IngredientResponse)
│   ├── DailyLogs/                        (GetOrCreate command, DailyLogResponse)
│   └── Auth/                             (Register command, Login command + validators)
├── TrainingNutrition.Api/
│   ├── DTOs/                             (CreateIngredientRequest, RegisterRequest, LoginRequest)
│   ├── Endpoints/                        (IngredientsEndpoints, DailyLogsEndpoints, AuthEndpoints)
│   ├── Exceptions/                       (GlobalExceptionHandler)
│   ├── BearerSecuritySchemeTransformer
│   └── Program.cs
└── TrainingNutrition.Tests/              (90 passing — unit + integration)
    ├── Integration/                      (CustomWebApplicationFactory, 3 integration test classes)
    └── Application/                      (handler unit tests with Moq)
```

---

## Phase Progress

### ✅ Phase 0 — Domain
Entities, Value Objects with validation in constructors. Unit tests for all domain types.

### ✅ Phase 1 — EF Core Foundations
Docker + PostgreSQL. AppDbContext. Fluent API entity configurations. Migrations. Basic CRUD.

### ✅ Phase 2 — Application Layer + Clean Architecture
Repository Pattern. MediatR Commands/Queries/Handlers. FluentValidation + Pipeline Behavior. Unit tests with Moq.

### ✅ Phase 3 — API Layer
Minimal API endpoints in `Endpoints/`. Swagger/OpenAPI + Scalar UI. GlobalExceptionHandler (Problem Details RFC 7807). Integration tests with WebApplicationFactory.

### ✅ Phase 4 — Auth
ASP.NET Core Identity setup. Register + Login endpoints. JWT generation (JwtSecurityTokenHandler). JWT middleware (AddAuthentication + AddJwtBearer). RequireAuthorization() on protected endpoints. Real UserId from claims in DailyLogsEndpoints.

### ✅ Phase 5 — Cross-cutting Concerns
GlobalExceptionHandler (ValidationException/InvalidOperationException → 400, catch-all → 500). Serilog structured logging + UseSerilogRequestLogging(). MediatR LoggingBehavior (measures total pipeline time). Redis + HybridCache in GetIngredientByIdHandler. NoOpHybridCache test double.

### ✅ Phase 6 — Production Readiness
Dockerfile multi-stage build (sdk:10.0 → aspnet:10.0-alpine, non-root user). Health checks (/health — PostgreSQL + Redis, JSON response, public). appsettings.Production.json + User Secrets for Jwt:Secret. GitHub Actions CI (postgres service container + Jwt__Secret from GitHub secret).

---

## API Endpoints

| Method | Route | Auth | Description |
|---|---|---|---|
| POST | /auth/register | No | Register user → 201 Guid |
| POST | /auth/login | No | Login → 200 JWT string |
| POST | /ingredients | Yes | Create ingredient → 201 Guid |
| GET | /ingredients/{id} | Yes | Get ingredient → 200 / 404 |
| GET | /dailylogs/{date} | Yes | Get or create daily log → 200 |
| GET | /health | No | Health check → JSON |

---

## CORS

`AllowFrontend` policy configured in `Program.cs` — allows `http://localhost:5173` (React dev server) with any header and any method. Added when connecting the frontend in Phase 7.

---

## Next Steps

- Backend Phases 0–6 ✅ complete
- Admin & Roles endpoints 📋 — see `ROADMAP_v0.1.md` (Admin & Roles section)
- v0.1 nutrition endpoints 📋 — see `ROADMAP_v0.1.md`
- Frontend driving which backend endpoints get built next — see `FRONTEND.md`
