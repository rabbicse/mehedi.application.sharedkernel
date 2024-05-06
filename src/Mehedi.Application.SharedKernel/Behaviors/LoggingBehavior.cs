using MediatR;
using Mehedi.Application.SharedKernel.Extensions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Mehedi.Application.SharedKernel.Behaviors;

/// <summary>
/// LoggingBehavior will be using on mediator events
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="logger"></param>
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    /// <summary>
    /// Handle will handle mediator requests
    /// It'll log the execution time of any command through mediatR
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var commandName = request.GetGenericTypeName();

        _logger.LogInformation("Handling command '{CommandName}'", commandName);

        var timer = new Stopwatch();
        timer.Start();

        var response = await next();

        timer.Stop();

        var timeTaken = timer.Elapsed.TotalSeconds;
        _logger.LogInformation("Command '{CommandName}' handled ({TimeTaken} seconds)", commandName, timeTaken);

        return response;
    }
}
