namespace Mehedi.Application.SharedKernel.BackgroundJobs;

/// <summary>
/// Queues work to run in the background, in this process, after the current request has returned.
/// </summary>
/// <remarks>
/// The default implementation (<see cref="ChannelBackgroundJobQueue"/>) keeps jobs in memory: it is fast and needs
/// no database, but jobs still queued when the process stops are lost. Use it for work that can be repeated or
/// recovered (re-sent by the client, rebuilt from the database); use a transactional outbox or a durable queue
/// for work that must never be lost.
/// </remarks>
public interface IBackgroundJobQueue
{
    /// <summary>Queues a job. Waits while the queue is full (back-pressure).</summary>
    /// <param name="name">A readable name for logs.</param>
    /// <param name="work">The work to run.</param>
    /// <param name="cancellationToken">Cancels waiting for space in a full queue.</param>
    /// <returns>The job id.</returns>
    ValueTask<Guid> EnqueueAsync(string name, BackgroundJobWork work, CancellationToken cancellationToken = default);
}
