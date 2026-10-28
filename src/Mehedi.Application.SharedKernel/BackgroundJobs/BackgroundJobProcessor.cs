using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mehedi.Application.SharedKernel.BackgroundJobs;

/// <summary>
/// Hosted service that runs queued <see cref="BackgroundJob"/>s, each in its own DI scope, with retries.
/// </summary>
public sealed partial class BackgroundJobProcessor(
    ChannelBackgroundJobQueue queue,
    IServiceScopeFactory scopeFactory,
    IOptions<BackgroundJobOptions> options,
    ILogger<BackgroundJobProcessor> logger) : BackgroundService
{
    private readonly BackgroundJobOptions _options = options.Value;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workers = Enumerable.Range(0, _options.MaxConcurrency)
            .Select(_ => Task.Run(() => WorkAsync(stoppingToken), CancellationToken.None))
            .ToArray();

        await Task.WhenAll(workers).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken).ConfigureAwait(false);

        if (queue.Count > 0)
        {
            LogDropped(logger, queue.Count);
        }
    }

    private async Task WorkAsync(CancellationToken stoppingToken)
    {
        try
        {
            await foreach (var job in queue.Reader.ReadAllAsync(stoppingToken).ConfigureAwait(false))
            {
                await RunAsync(job, stoppingToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Shutting down.
        }
    }

    private async Task RunAsync(BackgroundJob job, CancellationToken stoppingToken)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                var scope = scopeFactory.CreateAsyncScope();
                await using (scope.ConfigureAwait(false))
                {
                    await job.Work(scope.ServiceProvider, stoppingToken).ConfigureAwait(false);
                }

                LogSucceeded(logger, job.Name, job.Id, attempt);
                return;
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                LogInterrupted(logger, job.Name, job.Id);
                throw;
            }
            catch (Exception ex) when (attempt < _options.MaxAttempts)
            {
                LogRetrying(logger, ex, job.Name, job.Id, attempt, _options.MaxAttempts);
                await Task.Delay(_options.RetryDelay * attempt, stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // A failing job must not stop the processor.
                LogFailed(logger, ex, job.Name, job.Id, attempt);
                return;
            }
        }
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Background job {JobName} {JobId} succeeded (attempt {Attempt})")]
    private static partial void LogSucceeded(ILogger logger, string jobName, Guid jobId, int attempt);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Background job {JobName} {JobId} failed (attempt {Attempt} of {MaxAttempts}); retrying")]
    private static partial void LogRetrying(ILogger logger, Exception exception, string jobName, Guid jobId, int attempt, int maxAttempts);

    [LoggerMessage(Level = LogLevel.Error, Message = "Background job {JobName} {JobId} failed after {Attempts} attempt(s) and was given up")]
    private static partial void LogFailed(ILogger logger, Exception exception, string jobName, Guid jobId, int attempts);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Background job {JobName} {JobId} was interrupted by shutdown")]
    private static partial void LogInterrupted(ILogger logger, string jobName, Guid jobId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Shutting down with {Count} background job(s) still queued; they are lost")]
    private static partial void LogDropped(ILogger logger, int count);
}
