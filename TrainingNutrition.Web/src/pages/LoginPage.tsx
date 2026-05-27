import { useState } from "react";
import { useNavigate } from "react-router";
import apiClient from "@/api/client";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { isAxiosError } from "axios"

function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const navigate = useNavigate();

  async function handleSubmit(e: React.SubmitEvent<HTMLFormElement>) {
    e.preventDefault();

    try {
      const response = await apiClient.post<{ token: string }>("/auth/login", {
        email,
        password,
      });

      localStorage.setItem("token", response.data.token);

      navigate("/");
      
    } catch (error) {
      if (isAxiosError(error)) {
        setError(error.response?.data?.detail ?? "Login failed");
      } else {
        setError("Login failed");
      }
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Login</CardTitle>
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
            <Button type="submit">Login</Button>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}

export default LoginPage;
