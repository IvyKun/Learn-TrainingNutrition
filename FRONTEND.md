# TrainingNutrition — Frontend React Plan

## Goal

Build a React frontend that consumes the existing .NET API, enabling real interaction with the application: register, log in, manage ingredients, and record daily nutrition. The frontend drives backend development — new endpoints are added as the UI needs them.

This is a portfolio project. The stack reflects what .NET full-stack job offers in 2026 actually require.

---

## Stack

| Technology | Role | Why |
|---|---|---|
| **React 19** | UI framework | Most demanded in .NET full-stack offers |
| **TypeScript** | Typed JavaScript | Mandatory in professional environments; natural for C# developers |
| **Vite** | Build tool | Current standard — Create React App is deprecated |
| **React Router v6** | Client-side navigation | Pages, protected routes, layout nesting |
| **Axios** | HTTP client | API calls with JWT token in Authorization header |
| **TanStack Query** | Server state management | Fetch, cache, loading/error states — one library replaces manual useEffect chains |
| **Tailwind CSS** | Styling | Utility-first — fast to write, consistent, no separate CSS files |
| **shadcn/ui** | Component library | Built on Tailwind — tables, modals, forms, dropdowns, ready to use |
| **React Hook Form** | Form management | Controlled forms without boilerplate |
| **Zod** | Schema validation | Typed validation that pairs with React Hook Form and mirrors FluentValidation on the backend |

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
- Install React Router
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

Steps in order. Each step is one session. Do not skip ahead.

### Prerequisites
- 📋 **Step 0** — Install Node.js LTS on the machine; verify `node --version` and `npm --version`

### Phase 1 — Project setup
- 📋 **Step 1** — Create `TrainingNutrition.Web/` with Vite (`npm create vite@latest`)
- 📋 **Step 2** — Understand `package.json`, `tsconfig.json`, `vite.config.ts` — what each file controls
- 📋 **Step 3** — Install all dependencies (React Router, Axios, TanStack Query, Tailwind, shadcn/ui, React Hook Form, Zod)
- 📋 **Step 4** — Set up the folder structure (`src/api/`, `src/components/`, `src/pages/`, `src/hooks/`, `src/types/`)

### Phase 2 — First component
- 📋 **Step 5** — Build a static component in TypeScript — understand what a component is, props, `useState`
- 📋 **Step 6** — Render a static page (no API calls yet)

### Phase 3 — Routing
- 📋 **Step 7** — Add React Router: `BrowserRouter`, `Routes`, `Route`
- 📋 **Step 8** — Create empty pages: Login, Register, Dashboard
- 📋 **Step 9** — Add protected routes — redirect to `/login` if no JWT in `localStorage`

### Phase 4 — API client + auth flow
- 📋 **Step 10** — Create the Axios instance pointing to `http://localhost:5119` with JWT interceptor
- 📋 **Step 11** — Build Register form → calls `POST /auth/register`
- 📋 **Step 12** — Build Login form → calls `POST /auth/login`, stores JWT in `localStorage`, redirects to dashboard

### Phase 5 — TanStack Query
- 📋 **Step 13** — Add `GET /ingredients` endpoint to the .NET backend (needed here for the first time)
- 📋 **Step 14** — `useQuery` to fetch and display the ingredient list
- 📋 **Step 15** — `useMutation` to create an ingredient — loading states, error states, cache invalidation

### Phase 6 — Forms with React Hook Form + Zod
- 📋 **Step 16** — Add Zod schema validation to the create ingredient form
- 📋 **Step 17** — Apply React Hook Form + Zod to Register and Login forms
- 📋 **Step 18** — Per-field error messages (mirrors backend FluentValidation)

### Phase 7 — shadcn/ui
- 📋 **Step 19** — Install and configure shadcn/ui on top of Tailwind
- 📋 **Step 20** — Replace raw HTML with `Button`, `Input`, `Card`, `Table`, `Dialog` components

### Phase 8 — Daily log flow
- 📋 **Step 21** — Add v0.1 backend endpoints: `POST /dailylogs/{date}/meals`, `POST /dailylogs/{date}/meals/{mealId}/dishes`, extended `GET /dailylogs/{date}`
- 📋 **Step 22** — Daily log page: display today's full breakdown (meals → dishes → entries)
- 📋 **Step 23** — Add meal form + add dish form
- 📋 **Step 24** — Date picker to navigate between days

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
