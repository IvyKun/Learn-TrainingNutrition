
using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace TrainingNutrition.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : notnull
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        logger.LogInformation("Executing {RequestName}", requestName);

        Stopwatch stopwatch = Stopwatch.StartNew();

        TResponse response = await next(cancellationToken);

        stopwatch.Stop();

        logger.LogInformation("Executed {RequestName} in {ElapsedMs}ms", requestName, stopwatch.ElapsedMilliseconds);

        return response;
    }
}