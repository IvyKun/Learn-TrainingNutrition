# CLAUDE.md — Development Standards

## Project Context

Nutrition tracking application built with Clean Architecture.

- **Backend:** .NET 10, ASP.NET Core Minimal APIs, EF Core + PostgreSQL, MediatR, FluentValidation, ASP.NET Core Identity, JWT, Redis + HybridCache, xUnit + Moq, Docker
- **Frontend:** React 19, TypeScript, Vite, React Router v7, Axios, TanStack Query, React Hook Form, Zod, Tailwind CSS v4, shadcn/ui

Read [BACKEND.md](BACKEND.md) for backend architecture and API reference. Read [FRONTEND.md](FRONTEND.md) for frontend architecture and build checklist.

---

## Code Standards

### Naming
- Classes, methods, properties: `PascalCase`
- Private fields: `_camelCase`
- Local variables, parameters: `camelCase`
- Interfaces: `IMyInterface`
- Async methods: suffix with `Async` — `GetUserAsync()`

### C# Best Practices
- Always use `async/await` for I/O — never `.Result` or `.Wait()`
- Always pass and respect `CancellationToken` in async methods
- Use `record` types for Value Objects and DTOs
- Prefer `IReadOnlyList<T>` over `List<T>` for return types
- Never use `var` when the type isn't obvious from the right side
- Nullable reference types enabled — handle nulls explicitly

### Architecture Rules
- No business logic in Controllers or Endpoints — ever
- No EF Core references in Application layer — ever
- No direct `new` for dependencies — always inject via constructor
- One class per file
- Keep methods short — if it needs a comment to explain what it does, consider extracting it

### Testing Rules
- Every Handler must have unit tests
- Tests follow **Arrange / Act / Assert** structure with comments
- Test names follow: `MethodName_StateUnderTest_ExpectedBehavior`
- Never test implementation details — test behavior

### Frontend Standards
- Double quotes throughout TypeScript/TSX — matches Prettier default and shadcn generated code
- Backtick only when interpolating variables with `${}`
- All identifiers and comments in English

---

## Working with Claude Code

Before making any changes:
1. Read `BACKEND.md` and `FRONTEND.md` — they describe the current state of the project
2. Follow the code standards above without deviation
3. Prefer editing existing files to creating new ones
4. Do not introduce abstractions or patterns beyond what the task requires
5. Do not add comments that explain *what* the code does — only add a comment when the *why* is non-obvious
