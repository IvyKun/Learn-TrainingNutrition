import apiClient from "./client"
import type { IngredientResponse } from "../types/ingredient"

async function getIngredients() {

    const response = await apiClient.get<IngredientResponse[]>("/ingredients", {});

      return response.data;
}

export default getIngredients