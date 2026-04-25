
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace TrainingNutrition.Api;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
       if(exception is ValidationException validationException)
        {
           httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
           await httpContext.Response.WriteAsJsonAsync(
                                    new { errors = validationException.Errors.Select(e => e.ErrorMessage) },
                                    cancellationToken);

           return true;
        }

         return false;
    }
}