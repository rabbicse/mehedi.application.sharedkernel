namespace Mehedi.Application.SharedKernel.BackgroundJobs;

/// <summary>Settings of the in-memory background job queue.</summary>
public sealed class BackgroundJobOptions
{
    /// <summary>Maximum number of queued jobs; <c>EnqueueAsync</c> waits when the queue is full. Default 1000.</summary>
    public int Capacity { get; set; } = 1000;

    /// <summary>How many jobs run at the same time. Default 1 (jobs run in the order they were queued).</summary>
    public int MaxConcurrency { get; set; } = 1;

    /// <summary>How many times a job is attempted before it is given up (and logged as failed). Default 3.</summary>
    public int MaxAttempts { get; set; } = 3;

    /// <summary>Delay before the first retry; attempt <c>n</c> waits <c>(n - 1) × RetryDelay</c>. Default 2 seconds.</summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);
}
