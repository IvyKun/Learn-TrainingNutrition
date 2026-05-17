# Etapa 1: compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar solo los .csproj primero (para cachear restore)
COPY TrainingNutrition.slnx .
COPY TrainingNutrition.Domain/TrainingNutrition.Domain.csproj TrainingNutrition.Domain/
COPY TrainingNutrition.Application/TrainingNutrition.Application.csproj TrainingNutrition.Application/
COPY TrainingNutrition.Infrastructure/TrainingNutrition.Infrastructure.csproj TrainingNutrition.Infrastructure/
COPY TrainingNutrition.Api/TrainingNutrition.Api.csproj TrainingNutrition.Api/

RUN dotnet restore TrainingNutrition.Api/TrainingNutrition.Api.csproj

# Copiar todo el código fuente y publicar
COPY . .
WORKDIR /src/TrainingNutrition.Api
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "TrainingNutrition.Api.dll"]