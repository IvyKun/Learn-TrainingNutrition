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