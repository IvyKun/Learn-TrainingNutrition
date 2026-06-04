
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";


import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import { getIngredients, createIngredient, deleteIngredient } from "@/api/ingredients";
import type { IngredientResponse, CreateIngredientRequest, UpdateIngredientRequest } from "@/types/ingredient"

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

const createIngredientSchema = z.object({
  name: z.string().min(1, "Name is required"),
  brand: z.string().nullable().optional(),
  protein: z.coerce.number({ error: "Must be a number" }).min(0, "Cannot be negative"),
  carbs: z.coerce.number({ error: "Must be a number" }).min(0, "Cannot be negative"),
  fat: z.coerce.number({ error: "Must be a number" }).min(0, "Cannot be negative"),
  fiber: z.coerce.number({ error: "Must be a number" }).min(0, "Cannot be negative"),
  salt: z.coerce.number({ error: "Must be a number" }).min(0, "Cannot be negative"),
});

type CreateIngredientForm = z.infer<typeof createIngredientSchema>;

function IngredientsPage() {

  const { data, isLoading, isError } = useQuery({
    queryKey: ["ingredients"],
    queryFn: getIngredients,
  });

  const queryClient = useQueryClient();

  const form = useForm<CreateIngredientForm>({
    resolver: zodResolver(createIngredientSchema),
    defaultValues: {
      name: "",
      brand: null,
      protein: 0,
      carbs: 0,
      fat: 0,
      fiber: 0,
      salt: 0,
    },
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteIngredient(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["ingredients"] });
    },
  });

  const createMutation = useMutation({
    mutationFn: (data: CreateIngredientForm) => createIngredient(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["ingredients"] });
      form.reset();
    },
  });


  if (isLoading) return <p>Cargando...</p>;
  if (isError) return <p>Error</p>;

  return (
    <div className="flex min-h-screen items-center justify-center">
      <Card className="w-full max-w-max p-8">
       <div>
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
              <td><Button type="button"
                          onClick={() => deleteMutation.mutate(ingredient.id)}
                          disabled={deleteMutation.isPending}
                          >Delete</Button></td>
            </tr>
          ))}
        </tbody>
        </table>

        <form onSubmit={form.handleSubmit((data) => createMutation.mutate(data))} className="p-8 flex flex-col gap-4">
          <h2 className="font-semibold">Add Ingredient</h2>
          <div className="flex flex-col gap-1">
            <label htmlFor="name" className="text-sm font-medium">Name</label>
            <Input id="name" placeholder="e.g. Chicken breast" {...form.register("name")} />
            {form.formState.errors.name && (
              <p className="text-sm text-red-500">{form.formState.errors.name.message}</p>
            )}
          </div>
           <div className="flex flex-col gap-1">
            <label htmlFor="brand" className="text-sm font-medium">Brand (optional)</label>
            <Input id="brand" placeholder="e.g. Mercadona" {...form.register("brand")} />
          </div>
          <div className="flex flex-col gap-1">
            <label htmlFor="fat" className="text-sm font-medium">Fat</label>
            <Input type="number" id="fat" placeholder="Fat" {...form.register("fat")} />
            {form.formState.errors.fat && (
              <p className="text-sm text-red-500">{form.formState.errors.fat.message}</p>
            )}
          </div>
          <div className="flex flex-col gap-1">
            <label htmlFor="carbs" className="text-sm font-medium">Carbs</label>
            <Input type="number" id="carbs" placeholder="Carbs" {...form.register("carbs")} />
            {form.formState.errors.carbs && (
              <p className="text-sm text-red-500">{form.formState.errors.carbs.message}</p>
            )}
          </div>
          <div className="flex flex-col gap-1">
            <label htmlFor="protein" className="text-sm font-medium">Protein</label>
            <Input type="number" id="protein" placeholder="Protein" {...form.register("protein")} />
            {form.formState.errors.protein && (
              <p className="text-sm text-red-500">{form.formState.errors.protein.message}</p>
            )}
          </div>
          <div className="flex flex-col gap-1">
            <label htmlFor="fiber" className="text-sm font-medium">Fiber</label>
            <Input type="number" id="fiber" placeholder="Fiber" {...form.register("fiber")} />
            {form.formState.errors.fiber && (
              <p className="text-sm text-red-500">{form.formState.errors.fiber.message}</p>
            )}
          </div>
          <div className="flex flex-col gap-1">
            <label htmlFor="salt" className="text-sm font-medium">Salt</label>
            <Input type="number" id="salt" placeholder="Salt" {...form.register("salt")} />
            {form.formState.errors.salt && (
              <p className="text-sm text-red-500">{form.formState.errors.salt.message}</p>
            )}
          </div>

          <Button type="submit" disabled={createMutation.isPending}>
            {createMutation.isPending ? "Saving..." : "Add Ingredient"}
          </Button>
        </form>

      </Card>
    </div>
  );
}

export default IngredientsPage;
