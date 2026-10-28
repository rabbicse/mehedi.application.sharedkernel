namespace Mehedi.Application.SharedKernel.BackgroundJobs;

/// <summary>The work a background job performs, given a fresh DI scope.</summary>
/// <param name="services">Services from a scope created for this job (scoped services such as a DbContext are safe to use).</param>
/// <param name="cancellationToken">Cancelled when the host shuts down.</param>
public delegate Task BackgroundJobWork(IServiceProvider services, CancellationToken cancellationToken);

/// <summary>A queued unit of work.</summary>
/// <param name="Id">Identifies the job in logs.</param>
/// <param name="Name">A readable name, for example the MediatR request type.</param>
/// <param name="Work">What to run.</param>
/// <param name="EnqueuedAt">When the job was queued (UTC).</param>
public sealed record BackgroundJob(Guid Id, string Name, BackgroundJobWork Work, DateTimeOffset EnqueuedAt);
