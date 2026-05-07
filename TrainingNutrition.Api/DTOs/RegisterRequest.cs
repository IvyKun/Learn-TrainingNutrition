namespace TrainingNutrition.Api.DTOs;

public sealed record RegisterRequest(
    string Email,
    string Password
);