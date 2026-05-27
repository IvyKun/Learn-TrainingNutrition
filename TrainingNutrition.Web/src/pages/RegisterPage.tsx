import { useState } from "react"
import { useNavigate } from "react-router"
import apiClient from "@/api/client"

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
  <div>
    <h1>Register</h1>

    {error && <p>{error}</p>}

    <form onSubmit={handleSubmit}>
      <input
        type="email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        placeholder="Email"
      />
      <input
        type="password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        placeholder="Password"
      />
      <button type="submit">Register</button>
    </form>
  </div>
)
}

export default RegisterPage