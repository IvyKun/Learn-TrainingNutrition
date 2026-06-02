import { useQuery } from "@tanstack/react-query";
import getIngredients from "@/api/ingredients";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

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
        <CardHeader>
          <CardTitle>Ingredients</CardTitle>
        </CardHeader>

        <CardContent>
          <ul>
            {data?.map((ingredient) => (
              <li key={ingredient.id}>{ingredient.name}</li>
            ))}
          </ul>
        </CardContent>
      </Card>
    </div>
  );
}

export default IngredientsPage;
