using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Mehedi.Application.SharedKernel.BackgroundJobs;

/// <summary>Registration and MediatR helpers for the in-memory background job queue.</summary>
public static class BackgroundJobServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IBackgroundJobQueue"/> (a bounded channel) and the hosted <see cref="BackgroundJobProcessor"/>.
    /// A lightweight, in-process replacement for Hangfire when jobs need no persistence or dashboard.
    /// </summary>
    /// <example>
    /// <code>
    /// builder.Services.AddChannelBackgroundJobs(options => options.MaxConcurrency = 4);
    /// </code>
    /// </example>
    public static IServiceCollection AddChannelBackgroundJobs(
        this IServiceCollection services,
        Action<BackgroundJobOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<BackgroundJobOptions>()
            .Configure(options => configure?.Invoke(options))
            .Validate(options => options.Capacity > 0, "BackgroundJobOptions.Capacity must be greater than zero.")
            .Validate(options => options.MaxConcurrency > 0, "BackgroundJobOptions.MaxConcurrency must be greater than zero.")
            .Validate(options => options.MaxAttempts > 0, "BackgroundJobOptions.MaxAttempts must be greater than zero.")
            .Validate(options => options.RetryDelay >= TimeSpan.Zero, "BackgroundJobOptions.RetryDelay cannot be negative.")
            .ValidateOnStart();

        services.TryAddSingleton(sp => new ChannelBackgroundJobQueue(
            sp.GetRequiredService<IOptions<BackgroundJobOptions>>().Value.Capacity,
            sp.GetService<TimeProvider>()));
        services.TryAddSingleton<IBackgroundJobQueue>(sp => sp.GetRequiredService<ChannelBackgroundJobQueue>());
        services.AddHostedService<BackgroundJobProcessor>();

        return services;
    }

    /// <summary>
    /// Queues a MediatR request (command) to be sent in the background, in a new DI scope, with the
    /// configured retries. The request's response, if any, is discarded.
    /// </summary>
    /// <example>
    /// <code>
    /// var jobId = await queue.EnqueueAsync(new CreateCustomerCommand { ... }, cancellationToken);
    /// return Accepted(new { jobId });
    /// </code>
    /// </example>
    public static ValueTask<Guid> EnqueueAsync(
        this IBackgroundJobQueue queue,
        IBaseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queue);
        ArgumentNullException.ThrowIfNull(request);

        return queue.EnqueueAsync(
            request.GetType().Name,
            (services, token) => services.GetRequiredService<ISender>().Send((object)request, token),
            cancellationToken);
    }
}
