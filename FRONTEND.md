# TrainingNutrition — Frontend React Plan

## Goal

Build a React frontend that consumes the existing .NET API, enabling real interaction with the application: register, log in, manage ingredients, and record daily nutrition. The frontend drives backend development — new endpoints are added as the UI needs them.

This is a portfolio project. The stack reflects what .NET full-stack job listings in 2026 actually require.

---

## Stack

| Technology | Version (May 2026) | Role | Why |
|---|---|---|---|
| **React** | 19.2.6 | UI framework | Most demanded in .NET full-stack offers |
| **TypeScript** | latest | Typed JavaScript | Mandatory in professional environments; natural for C# developers |
| **Vite** | 8.x | Build tool | Current standard — uses Rolldown (Rust bundler), 10-30x faster builds |
| **React Router** | v7.x | Client-side navigation | Pages, protected routes, layout nesting; v7 is stable and non-breaking upgrade from v6 |
| **Axios** | latest | HTTP client | API calls with JWT token in Authorization header |
| **TanStack Query** | 5.x | Server state management | Fetch, cache, loading/error states — one library replaces manual useEffect chains |
| **Tailwind CSS** | v4 | Styling | Utility-first — no tailwind.config.js, CSS-native variables, zero-config content detection |
| **shadcn/ui** | latest | Component library | Full support for Tailwind v4 + React 19 — tables, modals, forms, dropdowns, ready to use |
| **React Hook Form** | latest | Form management | Controlled forms without boilerplate |
| **Zod** | v4 | Schema validation | Stable as of May 19 2026 — 14x faster, 57% smaller bundle; pairs with React Hook Form |

---

## Learning Order

Each phase introduces one new concept. Do not skip ahead.

### Phase 1 — Project setup
- Create the Vite + React + TypeScript project
- Understand what each config file does (`vite.config.ts`, `tsconfig.json`, `package.json`)
- Install dependencies
- Folder structure

### Phase 2 — First component and TypeScript basics
- What is a component in React
- Props and their types
- `useState` — local component state
- Render a static page (no API yet)

### Phase 3 — Routing
- Install React Router v7
- `BrowserRouter`, `Routes`, `Route`
- Pages: Login, Register, Dashboard (empty)
- `Link` and `useNavigate` for navigation
- Protected routes — redirect to login if no token

### Phase 4 — API calls with Axios + auth flow
- What Axios is and how it differs from `fetch`
- Create an Axios instance with base URL and JWT interceptor
- `POST /auth/register` — register form
- `POST /auth/login` — login form, store JWT in `localStorage`
- Redirect after login

### Phase 5 — TanStack Query
- What server state is vs local state
- `useQuery` — fetch ingredients list (`GET /ingredients`)
- `useMutation` — create ingredient (`POST /ingredients`)
- Loading states, error states, cache invalidation

### Phase 6 — Forms with React Hook Form + Zod
- `useForm` hook
- Schema validation with Zod (mirrors backend FluentValidation)
- Error messages per field
- Apply to: create ingredient form, register form, login form

### Phase 7 — shadcn/ui components
- Install and configure shadcn/ui
- Replace raw HTML with `Button`, `Input`, `Card`, `Table`, `Dialog`
- Daily log page — full breakdown view (meals → dishes → entries)

### Phase 8 — Daily log flow
- `GET /dailylogs/{date}` — display today's log
- `POST /dailylogs/{date}/meals` — add a meal
- `POST /dailylogs/{date}/meals/{mealId}/dishes` — add a dish with ingredients
- Date picker to navigate between days

---

## Development Setup

The React app runs locally alongside the .NET backend. Docker is only used for production deployment.

```
Your machine during development:
├── .NET API     → http://localhost:5119   (dotnet run)
├── React/Vite   → http://localhost:5173   (npm run dev)
└── Docker       → PostgreSQL + Redis      (docker compose up)
```

Node.js must be installed locally (not in Docker). Download the LTS version from https://nodejs.org — it is a standard Windows installer. `npm` (the package manager, equivalent to NuGet) is included automatically.

---

## Build Checklist

Steps in order. Steps within each phase restart at 1. Do not skip ahead.

### Prerequisites
- ✅ **Step 1** — Install Node.js LTS on the machine; verify `node --version` and `npm --version`

### Phase 1 — Project setup ✅
- ✅ **Step 1** — Create `TrainingNutrition.Web/` with Vite (`npm create vite@latest`)
- ✅ **Step 2** — Understand the generated file structure (`index.html`, `main.tsx`, `App.tsx`, config files)
- ✅ **Step 3** — Install dependencies (React Router, Axios, TanStack Query, React Hook Form, Zod)
- ✅ **Step 4** — Set up folder structure (`src/api/`, `src/components/`, `src/pages/`, `src/hooks/`, `src/types/`)

### Phase 2 — First component
- 📋 **Step 1** — Build a static component in TypeScript — understand what a component is, props, `useState`
- 📋 **Step 2** — Render a static page (no API calls yet)

### Phase 3 — Routing
- ✅ **Step 1** — Add React Router v7: `BrowserRouter`, `Routes`, `Route`
- ✅ **Step 2** — Create empty pages: Login, Register, Dashboard
- ✅ **Step 3** — Add protected routes — redirect to `/login` if no JWT in `localStorage`

### Phase 4 — API client + auth flow
- ✅ **Step 1** — Create the Axios instance pointing to `http://localhost:5119` with JWT interceptor
  - `src/api/client.ts` — `axios.create({ baseURL })` + `interceptors.request.use` to attach JWT from `localStorage`
  - Double quotes throughout (matches Prettier default and shadcn)
- ✅ **Step 2** — Build Register form → calls `POST /auth/register`
  - `src/pages/RegisterPage.tsx` — controlled inputs (useState), async submit handler, navigate on success
  - `name` field removed — `RegisterRequest` only accepts `email` + `password`
  - Error display shows generic message — improvement planned (Step 4)
  - CORS added to `Program.cs` (`AllowFrontend` policy for `http://localhost:5173`) to unblock browser requests
  - Updated in Phase 7 Step 2 to use shadcn `Input`, `Button`, `Card` — centered layout with `flex min-h-screen items-center justify-center`
- ✅ **Step 3** — Build Login form → calls `POST /auth/login`, stores JWT in `localStorage`, redirects to dashboard
  - `src/pages/LoginPage.tsx` — shadcn Input+Button+Card, `post<{ token: string }>` typed, `localStorage.setItem("token", response.data.token)` before `navigate("/")`
- ✅ **Step 4** — Improve error display — `isAxiosError` guard + `error.response?.data?.detail` in both Register and Login
  - Backend fix: `GlobalExceptionHandler` sets `ProblemDetails = { Detail = exception.Message }`
  - Backend fix: Register endpoint — removed local try/catch, exceptions bubble to GlobalExceptionHandler
  - Backend fix: Login endpoint — `Results.Problem(detail: "Invalid credentials", statusCode: 401)` instead of empty `Results.Unauthorized()`

### Phase 5 — TanStack Query
- ✅ **Step 1** — Backend prerequisites:
  - ✅ Add `Brand` field to `Ingredient` entity — domain change + migration (`AddBrandToIngredient`); affects `POST /ingredients` and all ingredient responses
  - ✅ `GET /ingredients?search=` — returns all, or filtered by name/brand if search term provided
  - ✅ `PUT /ingredients/{id}` — edit ingredient (backend only; frontend UI comes later)
  - ✅ `DELETE /ingredients/{id}` — delete ingredient (backend only; frontend UI comes later)
- ✅ **Step 2** — `useQuery` to fetch and display the ingredient list
  - `src/types/ingredient.ts` — `IngredientResponse` type (camelCase, matches .NET JSON serialization)
  - `src/api/ingredients.ts` — `getIngredients()` using shared `apiClient`
  - `src/main.tsx` — wrapped with `QueryClientProvider` + `QueryClient`
  - `src/pages/IngredientsPage.tsx` — `useQuery({ queryKey: ["ingredients"], queryFn: getIngredients })`, renders list with `isLoading` / `isError` guards
  - `App.tsx` — `/ingredients` route added, protected
  - `DashboardPage.tsx` — link to `/ingredients` added
  - `LoginPage.tsx` — fixed token storage: `response.data` instead of `response.data.token` (backend returns plain string)
- ✅ **Step 3** — `useMutation` for create and delete; React Hook Form + Zod validation on create form
  - `src/types/ingredient.ts` — `CreateIngredientRequest` + `UpdateIngredientRequest` types added
  - `src/api/ingredients.ts` — `createIngredient`, `updateIngredient`, `deleteIngredient` functions added
  - `src/pages/IngredientsPage.tsx` — table + `useMutation` delete (invalidateQueries) + create form with RHF+Zod (schema, per-field error messages, `form.reset()` on success); `disabled={isPending}` on both buttons
  - `npm install react-hook-form zod @hookform/resolvers` — dependencies added
  - Zod v4 API: `{ error: "..." }` replaces v3 `{ invalid_type_error: "..." }`; `z.coerce.number()` converts HTML string inputs to numbers before validation
  - Macro order in form follows Spanish nutrition label order: Fat → Carbs → Protein → Fiber → Salt
- ✅ **Step 4** — Edit ingredient — full CRUD complete on frontend
  - `src/pages/IngredientsPage.tsx` — `useState<IngredientResponse | null>` for selection; `useEffect` to fill form on selection change; `updateMutation` with explicit `UpdateIngredientRequest` mapping; conditional `onSubmit` (create vs update); Edit button per row; Save/Cancel button area (Cancel only shown in edit mode)
  - TypeScript 6 + @hookform/resolvers 5.x: do NOT use explicit `<T>` generic on `useForm` — inference from `zodResolver` works, explicit generic leaves `TFieldValues` unresolved
  - `form.reset(values)` updates internal defaults by default — use `{ keepDefaultValues: true }` when loading edit values so `form.reset()` still resets to the original empty form
  - `SubmitHandler<T>` imported from `react-hook-form` — define handler as `const onSubmit: SubmitHandler<T>` outside JSX for clean separation
  - `step="0.01"` on number inputs — browser accepts 0, 1, or 2 decimal places
  - `type="button"` required on Cancel — without it, any button inside a `<form>` triggers submit by default

### Phase 6 — Forms with React Hook Form + Zod
- ✅ **Step 1** — Zod v4 schema + RHF on create ingredient form (done as part of Phase 5 Step 3)
- ✅ **Step 2** — Apply React Hook Form + Zod to Register and Login forms
  - `RegisterPage.tsx` — `useMutation` replaces manual try/catch; Zod schema (`z.email` + `z.string().min(8)`); `useForm` + `zodResolver`; `onSuccess` navigates to `/login`; `onError` handles server errors via `isAxiosError`
  - `LoginPage.tsx` — same pattern; `onSuccess(response)` receives `AxiosResponse<string>`, stores `response.data` as token in `localStorage`, navigates to `/`
- ✅ **Step 3** — Per-field error messages on Register and Login (mirrors backend FluentValidation)
  - Both forms show per-field errors in `text-sm text-red-500` below each input; server errors shown in a separate `<p>` above the form

### Phase 7 — shadcn/ui
- ✅ **Step 1** — Install and configure Tailwind v4 + shadcn/ui
  - `tailwindcss` + `@tailwindcss/vite` installed; plugin in `vite.config.ts`
  - `src/index.css` replaced with `@import "tailwindcss"` + shadcn CSS variables
  - Path alias `@/` → `src/` in `tsconfig.json` (root) + `tsconfig.app.json` + `vite.config.ts`; `"ignoreDeprecations": "6.0"` needed for TypeScript 6
  - `npx shadcn@latest init` — Radix library, Nova preset (Lucide + Geist)
  - `src/components/ui/button.tsx` + `src/lib/utils.ts` created; Button verified in browser
- 🔄 **Step 2** — Replace raw HTML with `Button`, `Input`, `Card`, `Table`, `Dialog` components
  - `npx shadcn@latest add input card` — `src/components/ui/input.tsx` + `src/components/ui/card.tsx` created
  - `--background` changed to `oklch(0.96 0 0)` in `index.css` — light grey background, Card remains white
  - `RegisterPage.tsx` updated: `Input`, `Button`, `Card`, `CardHeader`, `CardTitle`, `CardContent`; layout centered
  - `LoginPage.tsx` — ✅ built with shadcn `Input`, `Button`, `Card`; centered layout; same structure as RegisterPage (built directly with shadcn in Phase 4 Step 3)
  - `Table` + `Dialog` — will be added during Phase 5 when building the ingredients page (not a separate step)

### Phase 9 — Admin panel (requires backend Admin Roadmap complete first)
- 📋 **Step 1** — Admin page `/admin/users` — table with all users, protected by Admin role
- 📋 **Step 2** — Delete button per user → calls `DELETE /admin/users/{id}`

### Phase 8 — Daily log flow
- 📋 **Step 1** — Add v0.1 backend endpoints: `POST /dailylogs/{date}/meals`, `POST /dailylogs/{date}/meals/{mealId}/dishes`, extended `GET /dailylogs/{date}`
- 📋 **Step 2** — Daily log page: display today's full breakdown (meals → dishes → entries)
- 📋 **Step 3** — Add meal form + add dish form
- 📋 **Step 4** — Date picker to navigate between days

---

## Pages to Build

| Page | Route | Description |
|---|---|---|
| Login | `/login` | Email + password form, stores JWT, redirects to dashboard |
| Register | `/register` | Name + email + password form |
| Dashboard | `/` | Today's daily log summary — calories, macros |
| Daily Log | `/logs/:date` | Full breakdown: meals → dishes → ingredient entries |
| Ingredients | `/ingredients` | List with search, create new |
| Ingredient detail | `/ingredients/:id` | View and edit a single ingredient |

---

## Connection with Backend Roadmap

The backend ROADMAP_v0.1.md defines the endpoints needed. The frontend drives which ones get built first:

| Frontend feature | Backend endpoint needed |
|---|---|
| Ingredient list + search | `GET /ingredients?search=` |
| Add meal | `POST /dailylogs/{date}/meals` |
| Add dish | `POST /dailylogs/{date}/meals/{mealId}/dishes` |
| Full daily log view | `GET /dailylogs/{date}` extended response |

Backend endpoints are added on demand as the UI reaches each feature.

---

## Project Structure (target)

```
TrainingNutrition.Web/          ← React app lives here (separate from .NET projects)
├── src/
│   ├── api/                    ← Axios instance + API call functions
│   ├── components/             ← Reusable UI components (Button, FormField, etc.)
│   ├── pages/                  ← One file per route
│   ├── hooks/                  ← Custom hooks (useAuth, etc.)
│   ├── types/                  ← TypeScript types and Zod schemas
│   └── main.tsx                ← App entry point
├── index.html
├── vite.config.ts
├── tsconfig.json
└── package.json
```

---

## Future Extra — State Management (Zustand / Redux Toolkit)

Not needed for this application in its current form. TanStack Query handles server state (ingredients, daily logs). Auth token in `localStorage` + React Context is sufficient for the user session.

These libraries become relevant when:
- Multiple components need to share state that is not server data
- The app grows to a point where Context re-renders become a performance problem
- A large team needs a predictable, centralised state store

**Zustand** — lightweight, minimal boilerplate, increasingly popular in modern projects.  
**Redux Toolkit** — more structured, common in large enterprise projects, heavier setup.

These are worth learning as a concept once the core frontend is working. They will not be part of the initial build.

---

## Out of Scope (for this document)

- Azure deployment → see `AZURE_DEPLOYMENT.md`
- Next.js — not needed for a SPA consuming a REST API
- Testing — add after core features are working
