import { useState } from "react"
import { useNavigate } from "react-router"
import { useMutation } from "@tanstack/react-query";

import apiClient from "@/api/client"
import { isAxiosError } from "axios"

import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

import { useForm, type SubmitHandler } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";

const loginSchema = z.object({
  email: z.email({ error: "Invalid email" }),
  password: z.string().min(8, { error: "Min 8 characters" }),
})

type LoginForm = z.infer<typeof loginSchema>;

function LoginPage() {

   const navigate = useNavigate() 

  const [error, setError] = useState("");

  const form = useForm({
     resolver: zodResolver(loginSchema),
     defaultValues: {
       email: "",
       password: "",
     },
   });

  const loginMutation = useMutation({
      mutationFn: (data: LoginForm) => apiClient.post<string>("/auth/login", data),
      onSuccess: (response) => {
        localStorage.setItem("token", response.data);
        navigate("/");
      },
      onError: (error) => {
        if (isAxiosError(error)) {
          setError(error.response?.data?.detail ?? "Login failed")
        } else {
          setError("Login failed")
        }
      },
  })

  const onSubmit: SubmitHandler<LoginForm> = (data) => {
    loginMutation.mutate(data)
  }
  
  return (
    <div className="flex min-h-screen items-center justify-center">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Login</CardTitle>
        </CardHeader>

        <CardContent>

          {error && <p>{error}</p>}

          <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-4">
          <div className="flex flex-col gap-1">
            <label htmlFor="email" className="text-sm font-medium">Email</label>
            <Input id="email" placeholder="Email" {...form.register("email")} />
            {form.formState.errors.email && (
              <p className="text-sm text-red-500">{form.formState.errors.email.message}</p>
            )}
          </div>
          <div className="flex flex-col gap-1">
            <label htmlFor="password" className="text-sm font-medium">Password</label>
            <Input type="password" id="password" placeholder="Password" {...form.register("password")} />
            {form.formState.errors.password && (
              <p className="text-sm text-red-500">{form.formState.errors.password.message}</p>
            )}
          </div>
          <div className="flex gap-2">
            <Button type="submit" disabled={loginMutation.isPending}>Login</Button>
          </div>
        </form>

        </CardContent>
      </Card>
    </div>
  );
}

export default LoginPage;
