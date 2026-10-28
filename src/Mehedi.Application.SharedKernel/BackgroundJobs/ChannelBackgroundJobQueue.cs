using System.Threading.Channels;

namespace Mehedi.Application.SharedKernel.BackgroundJobs;

/// <summary>
/// <see cref="IBackgroundJobQueue"/> backed by a bounded <see cref="Channel{T}"/>. Jobs are read by
/// <see cref="BackgroundJobProcessor"/>.
/// </summary>
public sealed class ChannelBackgroundJobQueue : IBackgroundJobQueue
{
    private readonly Channel<BackgroundJob> _channel;
    private readonly TimeProvider _timeProvider;

    /// <summary>Creates a queue that holds at most <paramref name="capacity"/> jobs.</summary>
    public ChannelBackgroundJobQueue(int capacity, TimeProvider? timeProvider = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);

        _channel = Channel.CreateBounded<BackgroundJob>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>The reading side, used by the processor.</summary>
    public ChannelReader<BackgroundJob> Reader => _channel.Reader;

    /// <summary>The number of jobs waiting.</summary>
    public int Count => _channel.Reader.Count;

    /// <inheritdoc />
    public async ValueTask<Guid> EnqueueAsync(string name, BackgroundJobWork work, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(work);

        var job = new BackgroundJob(Guid.NewGuid(), name, work, _timeProvider.GetUtcNow());
        await _channel.Writer.WriteAsync(job, cancellationToken).ConfigureAwait(false);
        return job.Id;
    }

    /// <summary>Stops accepting jobs; the processor finishes the queued ones and then stops reading.</summary>
    public void Complete() => _channel.Writer.TryComplete();
}
