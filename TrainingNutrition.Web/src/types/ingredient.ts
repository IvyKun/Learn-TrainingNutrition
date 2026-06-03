export type IngredientResponse = {
    id: string;
    name: string;
    brand: string | null;
    protein: number;
    carbs: number;
    fat: number;
    fiber: number;
    salt: number;
    caloriesPer100g: number;
};

export type CreateIngredientRequest = {
    name: string;
    brand: string | null;
    protein: number;
    carbs: number;
    fat: number;
    fiber: number;
    salt: number;
};

export type UpdateIngredientRequest = {
    name: string;
    brand: string | null;
    protein: number;
    carbs: number;
    fat: number;
    fiber: number;
    salt: number;
};