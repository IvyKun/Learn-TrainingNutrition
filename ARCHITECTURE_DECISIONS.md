# Architecture Decisions — TrainingNutrition

This document records every meaningful technology and pattern decision made in this project.
It is written so that any reader — human or AI — can understand not just **what** was used,
but **why**, **what problem it solves**, and **what would break** if it were removed or replaced.

Use this document to:
- Understand the reasoning behind the architecture
- Prepare for technical interviews about this project
- Ask an AI to quiz you on any section

---

## Table of Contents

### Backend
1. [Clean Architecture](#1-clean-architecture)
2. [Domain Layer — Entities and Value Objects](#2-domain-layer--entities-and-value-objects)
3. [Repository Pattern](#3-repository-pattern)
4. [CQRS + MediatR](#4-cqrs--mediatr)
5. [FluentValidation + Pipeline Behavior](#5-fluentvalidation--pipeline-behavior)
6. [ASP.NET Core Identity + JWT](#6-aspnet-core-identity--jwt)
7. [EF Core + PostgreSQL](#7-ef-core--postgresql)
8. [Redis + HybridCache](#8-redis--hybridcache)
9. [Serilog Structured Logging](#9-serilog-structured-logging)
10. [GlobalExceptionHandler — Problem Details](#10-globalexceptionhandler--problem-details)
11. [Minimal APIs](#11-minimal-apis)
12. [Docker + Docker Compose](#12-docker--docker-compose)
13. [Health Checks](#13-health-checks)
14. [GitHub Actions CI](#14-github-actions-ci)
15. [xUnit + Moq + WebApplicationFactory](#15-xunit--moq--webapplicationfactory)

### Frontend
16. [React + Vite + TypeScript](#16-react--vite--typescript)
17. [React Router v7](#17-react-router-v7)
18. [Axios — HTTP Client](#18-axios--http-client)
19. [TanStack Query](#19-tanstack-query)
20. [Tailwind CSS v4](#20-tailwind-css-v4)
21. [shadcn/ui](#21-shadcnui)
22. [React Hook Form + Zod](#22-react-hook-form--zod)

---

## 1. Clean Architecture

### What it is
Clean Architecture is a way of organising code into layers with a strict rule: dependencies only
point inward. The innermost layer (Domain) knows nothing about the outside world. The outer
layers depend on the inner ones, never the reverse.

```
API (outermost)
  └── Application
        └── Domain (innermost — zero external dependencies)
Infrastructure
  └── Application
        └── Domain
```

### What problem it solves
Without an architecture like this, business logic ends up scattered everywhere: in controllers,
in database queries, in UI code. When you want to change the database from PostgreSQL to MySQL,
or swap your API framework, you have to rewrite half the application because everything is tangled.

Clean Architecture creates clear boundaries. You can replace Infrastructure (the database, the
caching system, the identity provider) without touching a single line of business logic.

### Why we chose it
This project is a learning vehicle for enterprise .NET development. Clean Architecture is the
dominant pattern in professional .NET shops. Understanding it is a prerequisite for working on
any serious .NET backend team.

### SOLID principle
**Dependency Inversion Principle (DIP)** — high-level modules (Application) do not depend on
low-level modules (Infrastructure). Both depend on abstractions (interfaces defined in
Application). Application says "I need an `IIngredientRepository`"; Infrastructure provides
the concrete EF Core implementation.

### What breaks if you remove it
If you reference EF Core directly in a Controller, you cannot unit-test that controller without
a real database. If you put business logic in a Controller, you cannot reuse it from a
background job or a different entry point. Testability collapses immediately.

### Interview questions
- "Why did you choose Clean Architecture over MVC?"
- "What is the Dependency Rule and why does it matter?"
- "How would you swap PostgreSQL for SQL Server in your project?"
- "Can your Application layer be tested without a database? How?"

---

## 2. Domain Layer — Entities and Value Objects

### What it is
The Domain layer contains the business model: the things the application is actually about.
It has two building blocks:

**Entities** — objects with identity. `Ingredient` is an entity: two ingredients are the
same if they have the same `Id`, regardless of whether their names are identical.

**Value Objects** — objects defined entirely by their value, with no identity of their own.
`Email` is a value object: two emails are equal if they contain the same string. They are
immutable records.

### What problem it solves
Without Value Objects, validation is scattered. Every place that uses an email address would
have its own `if (email.Contains("@"))` check, and every place would be slightly different.
By wrapping email in an `Email` record with validation in its constructor, you guarantee that
any `Email` instance in the system is always valid — it cannot exist in an invalid state.

### Implementation choices
- All entities use `sealed class` — prevents accidental inheritance that breaks EF Core mapping
- All entities have `Guid Id { get; private init; } = Guid.NewGuid()` — the domain generates
  its own identity, not the database
- Private parameterless constructor on entities with navigation properties — required for EF Core
  to materialise (reconstruct) entities from the database
- Value Objects use `record` — records have structural equality by default in C#, which is
  exactly what a Value Object needs
- `Macronutrients` value object: Protein, Carbs, Fat, Fiber, Salt are stored; Calories is a
  computed property (`Protein * 4 + Carbs * 4 + Fat * 9`) — never stored in the database
  because it is always derivable and storing it would create a consistency risk

### SOLID principle
**Single Responsibility Principle (SRP)** — each Value Object is responsible for one concept
and its validation. `Grams` knows what a valid gram value is. `Email` knows what a valid email
is. Neither leaks into the other.

### Interview questions
- "What is the difference between an Entity and a Value Object?"
- "Why does your Email type validate itself in the constructor?"
- "Why is Calories a computed property and not stored in the database?"
- "Why do your entities use private `init` setters instead of public setters?"

---

## 3. Repository Pattern

### What it is
A Repository is an interface that acts as a collection of domain objects. Instead of writing
`dbContext.Ingredients.Where(...)` directly in business logic, the Application layer calls
`_ingredientRepository.GetByIdAsync(id)`. The concrete implementation lives in Infrastructure.

### What problem it solves
If Application directly references EF Core (`DbContext`), you have two problems:
1. You cannot unit-test handlers without a real database — EF Core is hard to mock reliably
2. If you ever change the data access technology, you have to touch Application code

The Repository abstracts the data source entirely. In tests, you provide a fake repository
(via Moq). In production, you provide the EF Core implementation. Application does not know
or care which one it gets.

### What we defined
- `IIngredientRepository` and `IDailyLogRepository` in `Application/Abstractions/`
- `EfIngredientRepository` and `EfDailyLogRepository` in `Infrastructure/Repositories/`
- Registered in DI container: `services.AddScoped<IIngredientRepository, EfIngredientRepository>()`

### SOLID principles
**Dependency Inversion Principle (DIP)** — Application depends on `IIngredientRepository`
(abstraction), not on `EfIngredientRepository` (concrete class).
**Interface Segregation Principle (ISP)** — we have two separate repository interfaces instead
of one `IRepository` with methods for everything. Each interface is small and focused.

### Interview questions
- "Why do you have a repository interface if EF Core is already an abstraction?"
- "How does the Repository Pattern enable unit testing?"
- "Where are repository interfaces defined — Application or Infrastructure? Why?"

---

## 4. CQRS + MediatR

### What it is
**CQRS** (Command Query Responsibility Segregation) is the principle that reads and writes are
different operations and should be modelled separately.

A **Command** changes state and returns a result (e.g., `CreateIngredientCommand` → returns the
new `Guid`). A **Query** reads state and returns data without changing anything (e.g.,
`GetIngredientByIdQuery` → returns `IngredientResponse`).

**MediatR** is the library that implements this. Instead of calling a service method directly,
a Controller sends a message (`mediator.Send(command)`). MediatR finds the registered handler
for that message and calls it.

### What problem it solves
Without CQRS, you end up with "service classes" that grow indefinitely: `IngredientService`
accumulates 20 methods over time. Every change to that class risks breaking unrelated
functionality. Methods that read data and methods that write data live together and share
dependencies they do not all need.

CQRS splits each operation into its own class with its own dependencies. Adding a new feature
means adding a new `Command` + `Handler` — you never touch existing code.

### What we implemented
- Commands: `CreateIngredientCommand`, `GetOrCreateDailyLogCommand`, `RegisterCommand`, `LoginCommand`
- Queries: `GetIngredientByIdQuery`
- Handlers: one per command/query, in `Application/Ingredients/`, `Application/DailyLogs/`, `Application/Auth/`

### SOLID principles
**Single Responsibility Principle (SRP)** — each handler has exactly one responsibility.
**Open/Closed Principle (OCP)** — to add a new operation, you add a new handler. You do not
modify any existing class.

### Why MediatR specifically
MediatR also enables the Pipeline Behavior pattern (see next section), which is where
cross-cutting concerns like validation and logging are handled without touching any handler.

### Interview questions
- "What is CQRS and why did you use it?"
- "What would happen to your codebase if you put all ingredient operations in one service?"
- "How does MediatR decouple the API layer from the Application layer?"
- "What is the difference between a Command and a Query in your architecture?"

---

## 5. FluentValidation + Pipeline Behavior

### What it is
**FluentValidation** is a library for defining validation rules as classes. Instead of writing
`if (request.Name == null || request.Name.Length > 200)` inside a handler, you write:
```csharp
RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
```

A **Pipeline Behavior** (MediatR concept) is middleware for the MediatR pipeline. Every
`mediator.Send()` call passes through all registered behaviors before reaching the handler.
`ValidationBehavior` intercepts every command, runs its validator, and throws a
`ValidationException` if invalid — before the handler even executes.

### What problem it solves
Without Pipeline Behavior, every handler would need to call `validator.Validate(request)` at
the top. That is repetitive code in every handler — a cross-cutting concern that should be
applied automatically, not manually. If a developer forgets to add validation in a new handler,
invalid data reaches business logic.

With `ValidationBehavior`, validation is guaranteed for every command, automatically, without
any handler having to remember to call it.

### What we implemented
- `ValidationBehavior<TRequest, TResponse>` in `Application/Behaviors/`
- `LoggingBehavior<TRequest, TResponse>` in `Application/Behaviors/` — measures total pipeline time
- Validators for: `CreateIngredientCommand`, `RegisterCommand`, `LoginCommand`

### SOLID principles
**Open/Closed Principle (OCP)** — Pipeline Behaviors extend the request-handling pipeline without
modifying any existing handler. Adding a new behavior (e.g., a rate-limiting behavior) requires
zero changes to existing code.
**Single Responsibility Principle (SRP)** — handlers are not responsible for validation.
Validators are not responsible for business logic.

### Interview questions
- "What is a Pipeline Behavior and what problem does it solve?"
- "What happens if a command fails validation in your system?"
- "Why not just validate inside each handler?"
- "How does LoggingBehavior measure the time of an operation?"

---

## 6. ASP.NET Core Identity + JWT

### What it is
**ASP.NET Core Identity** is Microsoft's framework for user management: storing users, hashing
passwords, verifying credentials. It provides `UserManager<T>`, `SignInManager<T>`, and integrates
directly with EF Core. It handles password hashing with PBKDF2 — you never store plain passwords.

**JWT** (JSON Web Token) is a self-contained token format for authentication. After login, the
server signs a token with a secret key and returns it to the client. Every subsequent request
includes this token in the `Authorization: Bearer <token>` header. The server verifies the
signature — no database lookup needed per request.

### What problem JWT solves
The alternative is session-based auth: the server stores session state, the client sends a
session cookie. This is stateful — it does not scale horizontally (requests must go to the same
server instance). JWT is stateless: any server that knows the secret can verify any token.
For a REST API designed to be deployed in Docker/Kubernetes, stateless auth is the correct choice.

### What we implemented
- `AppUser : IdentityUser` in Infrastructure — extends the Identity user with any custom fields
- `IdentityService` in `Infrastructure/Identity/` — wraps `UserManager` and `JwtSecurityTokenHandler`
- `IIdentityService` in `Application/Abstractions/` — Application depends on this abstraction, not on ASP.NET Core Identity directly
- JWT claims include `UserId` — endpoints read `context.User.FindFirst(ClaimTypes.NameIdentifier)` to know which user is making the request
- `RequireAuthorization()` on protected endpoints — returns 401 if no valid token

### SOLID principle
**Dependency Inversion Principle (DIP)** — Application uses `IIdentityService`. The concrete
`IdentityService` (which references ASP.NET Core Identity and JwtSecurityTokenHandler) lives
in Infrastructure. Application has no reference to ASP.NET Core Identity packages.

### Interview questions
- "Why did you choose JWT over session-based authentication?"
- "What is a JWT claim and how do you use one in your project?"
- "How do you know which user is making an API request?"
- "Where is password hashing implemented in your project? Did you write it yourself?"

---

## 7. EF Core + PostgreSQL

### What it is
**EF Core** (Entity Framework Core) is Microsoft's ORM (Object-Relational Mapper). It maps
C# classes to database tables, generates SQL, executes queries, and tracks changes. You write
C# — EF Core handles the SQL.

**Fluent API** (as opposed to Data Annotations) is how we configure the mapping: column names,
nullability, constraints, relationships. Configuration lives in separate `IEntityTypeConfiguration<T>`
classes, not as attributes on domain entities.

**PostgreSQL** is an open-source relational database, chosen because it is production-standard,
runs natively in Docker, and is the most common choice in modern .NET deployments.

### What problem it solves
Without an ORM, you write raw SQL strings everywhere, handle connection management manually,
and map query results to objects by hand. This is error-prone and tedious. EF Core automates
all of this while letting you drop to raw SQL for performance-critical queries when needed.

### Why Fluent API over Data Annotations
Data Annotations put infrastructure concerns (`[MaxLength(200)]`, `[Column("name")]`) directly
on Domain entities. The Domain layer should know nothing about the database. Fluent API
configurations live in Infrastructure — Domain entities stay clean.

### SOLID principle
**Single Responsibility Principle (SRP)** — each `IEntityTypeConfiguration<T>` class is
responsible only for mapping one entity type to the database. They do not contain queries or business logic.

### Interview questions
- "What is an ORM? What are the tradeoffs?"
- "Why did you use Fluent API instead of Data Annotations?"
- "What is an EF Core migration?"
- "Where are your EF Core configurations defined and why are they not in the Domain layer?"

---

## 8. Redis + HybridCache

### What it is
**Redis** is an in-memory data store used as a distributed cache. When a piece of data is
expensive to retrieve (database query, external API call), you store the result in Redis with
a TTL (time-to-live). The next request reads from Redis in microseconds instead of querying
the database.

**HybridCache** (introduced in .NET 9, used in .NET 10) is Microsoft's two-level cache abstraction:
L1 is in-process memory (instant, zero network), L2 is Redis (shared across multiple server
instances). It handles stampede protection — if 100 requests arrive for the same uncached key
simultaneously, only one database query is made.

### What we cached
`GetIngredientByIdHandler` — ingredient data changes rarely but is read frequently. The handler
calls `_cache.GetOrCreateAsync(cacheKey, ...)`. First call: database hit + cache write. Subsequent
calls within the TTL: cache hit, no database query.

### What problem it solves
Without caching, every `GET /ingredients/{id}` hits the database. Under load, this creates
unnecessary pressure on PostgreSQL for data that does not change between requests. Caching
decouples read performance from database throughput.

### How it is tested
Tests use `NoOpHybridCache` — a test double that never caches. This prevents tests from
interfering with each other through shared cached state.

### Interview questions
- "What is a distributed cache and why is it different from in-memory cache?"
- "What is cache stampede and how does HybridCache prevent it?"
- "Why not cache everything?"
- "How did you handle caching in unit tests?"

---

## 9. Serilog Structured Logging

### What it is
**Serilog** is a structured logging library. Instead of `Console.WriteLine("User {id} created")`,
you write `Log.Information("User {UserId} created", userId)`. The property `UserId` is stored
as a named field in the log event — not just concatenated into a string.

**Structured logging** means log events are queryable objects, not plain text. In a log aggregation
system (Seq, Elasticsearch, Datadog), you can filter by `UserId = "abc"` — impossible with plain text logs.

### What we added
- `UseSerilogRequestLogging()` — automatically logs every HTTP request with method, path, status
  code, and duration
- `LoggingBehavior` in the MediatR pipeline — logs the name of every request and how long the
  entire pipeline took to execute

### What problem it solves
`Console.WriteLine` produces unstructured text that is impossible to search at scale. In
production, you need to answer "how many 500 errors happened in the last hour?" or "what was
the response time for `/ingredients` today?" — structured logs make this possible.

### Interview questions
- "What is the difference between structured and unstructured logging?"
- "Where does your LoggingBehavior sit in the pipeline and what does it measure?"
- "Why not just use `Console.WriteLine`?"

---

## 10. GlobalExceptionHandler — Problem Details

### What it is
`GlobalExceptionHandler` is a class that implements `IExceptionHandler`. It intercepts all
unhandled exceptions from anywhere in the request pipeline and converts them into HTTP responses.

**Problem Details** (RFC 7807) is an IETF standard format for HTTP error responses:
```json
{ "type": "...", "title": "...", "status": 400, "detail": "...", "traceId": "..." }
```
Every API error, regardless of origin, returns this same structure.

### What problem it solves
Without a global handler, exceptions propagate up and either crash the request with a 500 and a
raw exception stack trace (a security risk in production — you never expose stack traces to clients),
or each endpoint has its own `try/catch` with inconsistent error formats.

GlobalExceptionHandler centralises error handling:
- `ValidationException` → 400 Bad Request with field-level error details
- `InvalidOperationException` → 400 Bad Request
- Anything else → 500 Internal Server Error (with no internal detail exposed to the client)

### SOLID principle
**Single Responsibility Principle (SRP)** — exception handling is not the responsibility of
individual endpoints or handlers. It is a cross-cutting concern handled in one place.

### Interview questions
- "What is Problem Details and why is it a standard?"
- "What happens in your API if a handler throws an unexpected exception?"
- "Why should you never return a stack trace to an API client?"

---

## 11. Minimal APIs

### What it is
Minimal APIs (introduced in .NET 6, refined in .NET 10) define HTTP endpoints with minimal
boilerplate — no `[ApiController]` classes, no `[HttpGet]` attributes. Endpoints are registered
directly in the DI container using extension methods:
```csharp
app.MapGet("/ingredients/{id}", async (...) => { ... });
```

Endpoint groups are organised in separate classes (`IngredientsEndpoints`, `AuthEndpoints`, etc.)
that extend the Minimal API routing using `IEndpointRouteBuilder` extension methods.

### Why not Controllers
Controllers are the traditional ASP.NET approach. Minimal APIs are lighter, have lower overhead,
and are now the Microsoft-recommended default for new APIs. For a REST API with few endpoints
per resource, Minimal APIs are cleaner. For a large API with complex routing needs, Controllers
remain valid.

### Interview questions
- "What are Minimal APIs and how are they different from Controllers?"
- "How did you organise your endpoints to avoid putting everything in Program.cs?"

---

## 12. Docker + Docker Compose

### What it is
**Docker** packages an application and all its dependencies into an image — a portable,
self-contained unit that runs identically on any machine.

**Multi-stage build** — the Dockerfile uses two stages:
1. `sdk:10.0` — the full .NET SDK to compile and publish the application
2. `aspnet:10.0-alpine` — a minimal Alpine Linux image that only contains the runtime (no compiler,
   no SDK). The published output is copied from stage 1 into stage 2.

Result: the final image is ~100MB instead of ~800MB. It has a minimal attack surface (less code
= fewer vulnerabilities).

**Docker Compose** runs multiple containers together:
- `trainingnutrition_db` — PostgreSQL 17
- `trainingnutrition_redis` — Redis 7 Alpine
- `trainingnutrition-api-1` — the .NET API

### What problem it solves
Without Docker, setting up the project on a new machine requires installing PostgreSQL,
configuring it, installing Redis, setting environment variables — 30 minutes of manual work that
is different on every OS. With `docker compose up -d`, the entire infrastructure is running in
seconds, reproducibly, on any machine.

### Interview questions
- "What is the purpose of a multi-stage Docker build?"
- "Why run the API as a non-root user inside the container?"
- "What is Docker Compose and what does it replace?"

---

## 13. Health Checks

### What it is
A health check endpoint (`GET /health`) reports whether the application and all its dependencies
are working. It checks:
- PostgreSQL connectivity (can we execute a query?)
- Redis connectivity (can we ping the cache?)

The response is JSON with a `status` field: `Healthy`, `Degraded`, or `Unhealthy`.

### What problem it solves
In production, load balancers and orchestration systems (Kubernetes, Azure App Service) call the
health endpoint periodically. If it returns `Unhealthy`, the platform stops routing traffic to
that instance and can restart it automatically. Without a health check, a broken instance
continues receiving requests it cannot fulfil.

### Interview questions
- "What is a health check endpoint used for in production?"
- "What does your health check verify?"

---

## 14. GitHub Actions CI

### What it is
**Continuous Integration (CI)** automatically runs tests every time code is pushed to the
repository. GitHub Actions is the CI platform built into GitHub.

Our workflow (`.github/workflows/ci.yml`):
1. Spins up a PostgreSQL service container — a real database, not a mock
2. Sets the JWT secret as a GitHub secret (not hardcoded in the workflow file)
3. Runs `dotnet test` — all 94 tests execute against real infrastructure

### What problem it solves
Without CI, broken code can be merged without anyone noticing until someone runs the tests
manually. CI makes it impossible to merge untested code — every push is verified automatically.

The PostgreSQL service container guarantees that integration tests run against a real database in CI,
not a mock. This is important: if the database schema migration is wrong, the CI pipeline fails.

### Interview questions
- "What is CI and why is it important?"
- "Why does your CI pipeline use a real PostgreSQL instead of an in-memory database?"
- "How do you handle secrets (like JWT_SECRET) in a CI pipeline?"

---

## 15. xUnit + Moq + WebApplicationFactory

### What it is
**xUnit** is the test framework for .NET. Tests are methods decorated with `[Fact]` (single case)
or `[Theory]` (parameterised).

**Moq** is a mocking library. It creates fake implementations of interfaces at runtime:
```csharp
var repo = new Mock<IIngredientRepository>();
repo.Setup(r => r.GetByIdAsync(id, ct)).ReturnsAsync(ingredient);
```
This lets you test a handler in complete isolation from the database.

**WebApplicationFactory** bootstraps the full ASP.NET Core application in memory for integration
tests. Integration tests make real HTTP requests to real endpoints against a real database
(PostgreSQL running in Docker locally, or as a service container in CI).

### Testing strategy in this project
- **Unit tests** (Moq): test individual handlers in isolation. Dependencies are mocked. Fast.
  Verify business logic.
- **Integration tests** (WebApplicationFactory): test the full stack end to end. Real HTTP →
  real middleware → real handler → real database. Verify that everything is wired together correctly.

### Interview questions
- "What is the difference between a unit test and an integration test?"
- "What does Moq do and why is it useful?"
- "What does WebApplicationFactory give you that a real deployed server does not?"
- "How do you test a 401 Unauthorized response?"

---

## 16. React + Vite + TypeScript

### What it is
**React** is a JavaScript UI library by Meta. The entire UI is built as a tree of **components** —
functions that return JSX (HTML-like syntax that compiles to JavaScript). When state changes,
React re-renders only the components that depend on that state.

**Vite** is the build tool and development server. It uses **Rolldown** (a Rust-based bundler)
which is 10–30x faster than the previous generation (Webpack). In development, it serves files
using ES modules natively — no full bundle rebuild on every change.

**TypeScript** is a superset of JavaScript with static types. For a C# developer, it is the
natural choice: you define interfaces and types, the compiler catches type errors before runtime.

### Why React for this project
React is the dominant framework in .NET full-stack job listings. It pairs naturally with a REST
API backend. TypeScript makes it approachable for a C# developer: the mental model of typed
objects, interfaces, and generics translates directly.

### Interview questions
- "What is a React component?"
- "Why TypeScript instead of plain JavaScript?"
- "What is Vite and why is it faster than Webpack?"
- "What is JSX?"

---

## 17. React Router v7

### What it is
React Router is the standard library for client-side routing in React. In a traditional web app,
every URL change triggers a full page reload from the server. With React Router, URL changes are
handled entirely in the browser — React swaps components without a page reload. This is called
a **Single Page Application (SPA)**.

**Protected routes** — a pattern where a route component checks if a JWT exists in `localStorage`
before rendering. If no token is found, it redirects to `/login`. This prevents unauthenticated
users from seeing protected pages.

### Key concepts
- `BrowserRouter` — wraps the app and listens to URL changes
- `Routes` + `Route` — declarative mapping from URL to component
- `useNavigate` — programmatic navigation (e.g., redirect after login)
- `Link` — renders an anchor tag that uses client-side navigation instead of a full reload

### Interview questions
- "What is a Single Page Application?"
- "How do protected routes work in your frontend?"
- "What is the difference between `useNavigate` and a regular `<a>` tag?"

---

## 18. Axios — HTTP Client

### What it is
Axios is an HTTP client for JavaScript/TypeScript. It wraps the browser's native `fetch` API
with a cleaner interface and adds crucial capabilities:

**Interceptors** — middleware for HTTP requests and responses. The JWT interceptor automatically
reads the token from `localStorage` and adds `Authorization: Bearer <token>` to every outgoing
request. Without this, every API call would need to set the header manually.

### Why Axios over native fetch
- Interceptors are not natively available in `fetch`
- Axios automatically parses JSON responses
- Error handling is more consistent — Axios throws on non-2xx status codes; `fetch` does not
- TypeScript support is cleaner

### Interview questions
- "What is an Axios interceptor and what does yours do?"
- "Where is the JWT token stored in your frontend?"
- "Why Axios instead of `fetch`?"

---

## 19. TanStack Query

### What it is
TanStack Query (formerly React Query) manages **server state** — data that comes from an API.
It handles:
- Fetching data (`useQuery`)
- Mutating data (`useMutation`)
- Caching, refetching, and invalidation
- Loading and error states

### What problem it solves
Without TanStack Query, every component that fetches API data needs:
```tsx
const [data, setData] = useState(null);
const [loading, setLoading] = useState(true);
const [error, setError] = useState(null);
useEffect(() => { fetch(...).then(...) }, []);
```
This is repetitive, error-prone, and does not handle cache invalidation (e.g., after creating
an ingredient, the ingredient list should automatically refresh).

TanStack Query replaces all of this with `useQuery` (two lines) and handles cache invalidation
automatically when mutations succeed.

### Server state vs local state
**Server state** = data that lives on the backend and is fetched via API (ingredients, daily logs,
user profile). Managed by TanStack Query.
**Local state** = UI state that only exists in the browser (modal open/closed, form input values).
Managed by `useState`.

### Interview questions
- "What is TanStack Query and what problem does it solve?"
- "What is the difference between server state and local state?"
- "What is cache invalidation and how does TanStack Query handle it after a mutation?"

---

## 20. Tailwind CSS v4

### What it is
Tailwind is a **utility-first CSS framework**. Instead of writing CSS classes like `.ingredient-card`
with custom rules, you compose utility classes directly in JSX:
```tsx
<div className="flex flex-col gap-4 rounded-lg bg-white p-6 shadow-md">
```

**v4 differences from v3:**
- No `tailwind.config.js` — configuration is done in CSS
- Uses CSS-native variables for the design system
- Zero-config content detection — Tailwind automatically finds all files to scan
- The Vite plugin (`@tailwindcss/vite`) replaces the PostCSS config

### What problem it solves
Traditional CSS requires switching between CSS files and HTML, naming classes, dealing with
specificity conflicts, and often duplicating styles. Tailwind keeps styling colocated with markup,
making components self-contained and predictable.

### Interview questions
- "What is utility-first CSS?"
- "What changed between Tailwind v3 and v4?"
- "Why not write regular CSS?"

---

## 21. shadcn/ui

### What it is
shadcn/ui is not a traditional component library installed as an npm package. Instead, it
copies source code directly into your project (`src/components/ui/`). You own the code — you
can modify any component.

Each component is built on **Radix UI** primitives (accessible, unstyled, behaviour-complete)
and styled with Tailwind utility classes. The **Nova preset** provides a design system with
Lucide icons and Geist font.

### Why not a traditional component library (MUI, Antd)
Traditional libraries give you pre-built components that you cannot easily customise deep down.
If the library's Button has a padding you want to change, you fight the library's CSS specificity.
shadcn gives you the source — you change the Tailwind class and you are done.

### Interview questions
- "How is shadcn/ui different from Material UI or Ant Design?"
- "What are Radix UI primitives?"
- "Why did you choose shadcn/ui for this project?"

---

## 22. React Hook Form + Zod

### What it is
**React Hook Form** manages forms in React without re-rendering the entire component on every
keystroke. It uses uncontrolled inputs (the DOM holds the value) and only triggers re-renders
when necessary (on validation errors, on submit).

**Zod** (v4) is a schema validation library. You define the shape and constraints of your data
as a TypeScript type and a runtime validator at the same time:
```typescript
const schema = z.object({
  email: z.string().email(),
  password: z.string().min(8),
});
```
This schema validates input and also infers the TypeScript type — no duplication.

### Why these two together
React Hook Form's `resolver` option accepts a Zod schema. The schema runs on submit (and
optionally on change). Type inference from the schema flows through the entire form — TypeScript
knows what `formData.email` is because Zod defined it.

### Mirror of backend FluentValidation
The same rules that FluentValidation enforces on the backend (`NotEmpty`, `MaximumLength`,
`EmailAddress`) are mirrored in Zod schemas on the frontend. This gives the user immediate
feedback without a round trip to the server.

### Interview questions
- "What is the difference between controlled and uncontrolled inputs in React?"
- "Why use Zod for frontend validation if the backend already validates?"
- "What does Zod's type inference give you?"
- "How does React Hook Form know when to show validation errors?"

---

## Summary — Technology Map

| Technology | Layer | What it replaces / What it adds |
|---|---|---|
| Clean Architecture | All | Ad-hoc layering; enables testability and replaceability |
| Value Objects | Domain | Scattered validation; ensures always-valid state |
| Repository Pattern | App ↔ Infra | Direct DbContext in business logic |
| CQRS + MediatR | Application | Fat service classes; enables pipeline behaviors |
| FluentValidation | Application | Manual if-checks in handlers |
| Pipeline Behavior | Application | Repeated validation/logging boilerplate in each handler |
| ASP.NET Core Identity | Infrastructure | Manual user + password storage |
| JWT | Infrastructure/API | Session-based auth; enables stateless horizontal scaling |
| EF Core + Fluent API | Infrastructure | Raw SQL; keeps Domain clean of DB concerns |
| HybridCache + Redis | Infrastructure | Repeated DB hits for unchanged data |
| Serilog | Infrastructure/API | Console.WriteLine; enables structured queryable logs |
| GlobalExceptionHandler | API | Try/catch in every endpoint; standardises error format |
| Minimal APIs | API | Controller boilerplate |
| Docker multi-stage | Deployment | Large images; reproducible environment setup |
| Health Checks | Deployment | No observability for load balancers |
| GitHub Actions CI | Process | Manual test execution; automated verification on every push |
| React + TypeScript | Frontend | Plain JS; type safety for a C# developer |
| Vite | Frontend | Webpack; dramatically faster build and HMR |
| React Router | Frontend | Full page reloads; SPA navigation with protected routes |
| Axios + interceptors | Frontend | Manual header injection on every API call |
| TanStack Query | Frontend | Manual useEffect + useState for server data |
| Tailwind v4 | Frontend | Custom CSS files; utility-first, colocated styling |
| shadcn/ui | Frontend | Black-box component libraries; owned, modifiable components |
| React Hook Form + Zod | Frontend | Uncontrolled form boilerplate; mirrors backend validation |
