import { Button } from '@/components/ui/button'

function App() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-100">
      <div className="flex flex-col items-center gap-4">
        <h1 className="text-4xl font-bold text-blue-600">TrainingNutrition</h1>
        <Button>Iniciar sesión</Button>
        <Button variant="outline">Registrarse</Button>
      </div>
    </div>
  )
}

export default App