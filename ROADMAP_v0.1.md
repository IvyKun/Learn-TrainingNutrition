# TrainingNutrition API — Roadmap v0.1

## Context

The current version (v0.0) covers the foundational backend: authentication, ingredient management, and daily log creation. The application is functional but incomplete for real daily use — a user cannot yet register what they actually ate on a given day.

This document lists the features required to make the API usable as a nutrition diary.

---

## What v0.1 Adds

### 1. List and Search Ingredients

**Why it's needed:** Before adding an ingredient to a meal, the user needs to discover what already exists. Currently there is no way to browse or search ingredients — only fetch one by exact ID.

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/ingredients` | Returns all ingredients |
| `GET` | `/ingredients?search=oat` | Returns all ingredients whose name contains the search text (case-insensitive) |

**Example response:**
```json
[
  { "id": "...", "name": "Oats", "caloriesPer100g": 379 },
  { "id": "...", "name": "Oat milk", "caloriesPer100g": 45 }
]
```

The search parameter is optional. If omitted, the full list is returned.

---

### 2. Add a Meal to a Daily Log

**Why it's needed:** A daily log is a container. Right now it can be created but nothing can be added to it. This endpoint lets the user say "I had Breakfast today".

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/dailylogs/{date}/meals` | Adds a meal to the daily log for that date |

**Request body:**
```json
{
  "name": "Breakfast",
  "type": "Breakfast",
  "occurredAt": "2026-05-17T08:30:00Z"
}
```

**`type` valid values:** `Breakfast`, `Lunch`, `Dinner`, `Snack`

**Response:** `201 Created` with the new meal ID.

---

### 3. Add a Dish to a Meal (with ingredients)

**Why it's needed:** A meal is composed of dishes. A dish is a named group of ingredients with their quantities in grams. For example, a Breakfast meal could contain one dish called "Oatmeal" with 80g of oats and 200ml of milk.

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/dailylogs/{date}/meals/{mealId}/dishes` | Adds a dish (with ingredients) to a meal |

**Request body:**
```json
{
  "name": "Oatmeal",
  "entries": [
    { "ingredientId": "...", "grams": 80 },
    { "ingredientId": "...", "grams": 200 }
  ]
}
```

**Response:** `201 Created` with the new dish ID.

---

### 4. Get Daily Log — Full Response

**Why it's needed:** The current `GET /dailylogs/{date}` response only returns `date` and `totalCalories`. To be useful as a diary, it needs to show the complete breakdown: meals → dishes → ingredients with quantities and calories.

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/dailylogs/{date}` | Returns the full daily log for the authenticated user |

**Example full response:**
```json
{
  "date": "2026-05-17",
  "totalCalories": 520,
  "totalProtein": 18.4,
  "totalCarbs": 72.1,
  "totalFat": 9.6,
  "meals": [
    {
      "id": "...",
      "name": "Breakfast",
      "type": "Breakfast",
      "occurredAt": "2026-05-17T08:30:00Z",
      "totalCalories": 520,
      "dishes": [
        {
          "id": "...",
          "name": "Oatmeal",
          "totalCalories": 520,
          "entries": [
            {
              "ingredientName": "Oats",
              "grams": 80,
              "calories": 303,
              "protein": 13.2,
              "carbs": 54.8,
              "fat": 5.6
            },
            {
              "ingredientName": "Milk",
              "grams": 200,
              "calories": 217,
              "protein": 7.0,
              "carbs": 10.2,
              "fat": 8.0
            }
          ]
        }
      ]
    }
  ]
}
```

The domain already computes all of these values (`GetTotalMacros()` exists on `DailyLog`, `Meal`, and `Dish`). Only the handler mapping and the response DTO need to be built.

---

## Summary

| # | Feature | Endpoint | Status |
|---|---|---|---|
| 1 | List ingredients | `GET /ingredients` | Missing |
| 2 | Search ingredients by name | `GET /ingredients?search=text` | Missing |
| 3 | Add meal to daily log | `POST /dailylogs/{date}/meals` | Missing |
| 4 | Add dish to meal | `POST /dailylogs/{date}/meals/{mealId}/dishes` | Missing |
| 5 | Daily log full response | `GET /dailylogs/{date}` (extended) | Partial — response too thin |

---

## User Flow Enabled by v0.1

```
Morning routine:

1. Search existing ingredients          GET /ingredients?search=oat
2. Create missing ingredient if needed  POST /ingredients
3. Open today's log                     GET /dailylogs/2026-05-17
4. Add a Breakfast meal                 POST /dailylogs/2026-05-17/meals
5. Add a dish with ingredients          POST /dailylogs/2026-05-17/meals/{id}/dishes
6. Review the full day                  GET /dailylogs/2026-05-17  ← now returns full breakdown
```

This covers the complete daily nutrition tracking loop.

---

---

# TrainingNutrition API — Roadmap v0.2

## Context

With v0.1 the user can log what they eat each day. v0.2 adds three optional health metrics to the daily log: body weight, sleep duration, and step count. These are independent from nutrition — each is updated via its own dedicated endpoint so the user can record them at any point during the day without touching the food data.

---

## What v0.2 Adds

### 1. Log Body Weight

**Why it's needed:** Weight tracking alongside nutrition is one of the most common use cases in any health diary. It is optional — not every day needs a weight entry.

| Method | Endpoint | Description |
|---|---|---|
| `PATCH` | `/dailylogs/{date}/weight` | Sets the body weight for that day |

**Request body:**
```json
{ "weightKg": 78.5 }
```

**Response:** `204 No Content`

Calling it again on the same day overwrites the previous value.

---

### 2. Log Sleep

**Why it's needed:** Sleep quality directly affects recovery and appetite. Recording it in the same daily log makes it easy to spot correlations over time.

| Method | Endpoint | Description |
|---|---|---|
| `PATCH` | `/dailylogs/{date}/sleep` | Sets the sleep duration for that day |

**Request body:**
```json
{ "sleepHours": 7.5 }
```

**Response:** `204 No Content`

---

### 3. Log Steps

**Why it's needed:** Daily activity (steps) is the simplest proxy for energy expenditure. Optional — rest days will simply have no entry.

| Method | Endpoint | Description |
|---|---|---|
| `PATCH` | `/dailylogs/{date}/steps` | Sets the step count for that day |

**Request body:**
```json
{ "steps": 9200 }
```

**Response:** `204 No Content`

---

## Impact on Daily Log Response

The three fields are added as nullable values to the full log response from v0.1. If the user has not recorded a value for a given day, the field is `null`.

```json
{
  "date": "2026-05-17",
  "totalCalories": 520,
  "weightKg": 78.5,
  "sleepHours": 7.5,
  "steps": 9200,
  "meals": [ ... ]
}
```

---

## Summary

| # | Feature | Endpoint | Notes |
|---|---|---|---|
| 1 | Log body weight | `PATCH /dailylogs/{date}/weight` | Optional, overwrites |
| 2 | Log sleep duration | `PATCH /dailylogs/{date}/sleep` | Optional, overwrites |
| 3 | Log step count | `PATCH /dailylogs/{date}/steps` | Optional, overwrites |
| 4 | Daily log response | `GET /dailylogs/{date}` | Extended with `weightKg`, `sleepHours`, `steps` (nullable) |
