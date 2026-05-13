
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace TrainingNutrition.Api.Exceptions;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    { 
         httpContext.Response.StatusCode = exception switch
         {
            ValidationException    => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            _                      => StatusCodes.Status500InternalServerError
         };

         return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
         {
            HttpContext = httpContext,
            Exception = exception
         });

    }


}