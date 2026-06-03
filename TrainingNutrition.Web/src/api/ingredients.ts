import apiClient from "@/api/client";
import type { IngredientResponse, CreateIngredientRequest, UpdateIngredientRequest } from "@/types/ingredient"

export async function getIngredients() {

    const response = await apiClient.get<IngredientResponse[]>("/ingredients");

    return response.data;
}

export async function createIngredient(data: CreateIngredientRequest) {

    const response = await apiClient.post<string>("/ingredients", data);

    return response.data;
}

export async function updateIngredient(id: string, data: UpdateIngredientRequest) {

    const response = await apiClient.put<string>(`/ingredients/${id}`, data);

    return response.data;
}

export async function deleteIngredient(id: string) {

    const response = await apiClient.delete<string>(`/ingredients/${id}`);

    return response.data;
}