import { useState } from "react"
import { useNavigate } from "react-router"
import apiClient from "@/api/client"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

function RegisterPage() {

  const [email, setEmail] = useState("")
  const [password , setPassword] = useState("")
  const [error, setError] = useState("")

  const navigate = useNavigate() 

  async function handleSubmit(e: React.SubmitEvent<HTMLFormElement>) {

    e.preventDefault()

    try{
      
      await apiClient.post("/auth/register", { email, password })

      navigate("/login")

    }catch{
      setError("Registration failed")
    }


    
  }

 return (
  <div className="flex min-h-screen items-center justify-center">
    <Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle>Register</CardTitle>
      </CardHeader>

      <CardContent>
        {error && <p>{error}</p>}

        <form onSubmit={handleSubmit} className="flex flex-col gap-4">
          <Input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="Email"
          />
          <Input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Password"
          />
          <Button type="submit">Register</Button>
        </form>
      </CardContent>

    </Card>

   
  </div>
)
}

export default RegisterPage