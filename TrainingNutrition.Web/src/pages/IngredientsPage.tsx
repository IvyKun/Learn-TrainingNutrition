import { useState } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { isAxiosError } from "axios"
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { getIngredients, createIngredient, deleteIngredient } from "@/api/ingredients";
import type { IngredientResponse, CreateIngredientRequest, UpdateIngredientRequest } from "@/types/ingredient"

function IngredientsPage() {

  const { data, isLoading, isError } = useQuery({
    queryKey: ["ingredients"],
    queryFn: getIngredients,
  });

  if (isLoading) return <p>Cargando...</p>;
  if (isError) return <p>Error</p>;
  
  return (
    <div className="flex min-h-screen items-center justify-center">
      <Card className="w-full max-w-sm">
       <div className="p-8">
          Ingredients
       </div>

         <table>

          <thead>
          <tr>
            <th scope="col">Name</th>
            <th scope="col">Brand</th>
            <th scope="col">kcal/100g</th>
            <th scope="col">Protein</th>
            <th scope="col">Carbs</th>
            <th scope="col">Fat</th>
            <th scope="col">Fiber</th>
            <th scope="col">Salt</th>
            <th scope="col"></th>
          </tr>
        </thead>
        <tbody>
            {data?.map((ingredient) => (
            <tr key={ingredient.id}>
              <td>{ingredient.name}</td>
              <td>{ingredient.brand}</td>
              <td>{ingredient.caloriesPer100g}</td>
              <td>{ingredient.protein}</td>
              <td>{ingredient.carbs}</td>
              <td>{ingredient.fat}</td>
              <td>{ingredient.fiber}</td>
              <td>{ingredient.salt}</td>
              <td><Button type="button">Delete</Button></td>
            </tr>
          ))}
        </tbody>
        </table>
      </Card>
    </div>
  );
}

export default IngredientsPage;
