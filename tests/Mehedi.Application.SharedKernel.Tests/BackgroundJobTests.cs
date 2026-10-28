using MediatR;
using Mehedi.Application.SharedKernel.BackgroundJobs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Mehedi.Application.SharedKernel.Tests;

public sealed class BackgroundJobTests : IAsyncLifetime
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
    private ServiceProvider _provider = null!;
    private IHostedService[] _hosted = [];

    private IBackgroundJobQueue Queue => _provider.GetRequiredService<IBackgroundJobQueue>();

    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<ScopedCounter>();
        services.AddSingleton<Recorder>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<BackgroundJobTests>());
        services.AddChannelBackgroundJobs(options =>
        {
            options.MaxAttempts = 3;
            options.RetryDelay = TimeSpan.Zero;
        });

        _provider = services.BuildServiceProvider();
        _hosted = _provider.GetServices<IHostedService>().ToArray();
        foreach (var service in _hosted)
        {
            await service.StartAsync(CancellationToken.None);
        }
    }

    public async Task DisposeAsync()
    {
        foreach (var service in _hosted)
        {
            await service.StopAsync(CancellationToken.None);
        }

        await _provider.DisposeAsync();
    }

    [Fact]
    public async Task A_queued_job_runs_in_the_background()
    {
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var id = await Queue.EnqueueAsync("test", (_, _) => { done.SetResult(); return Task.CompletedTask; });

        await done.Task.WaitAsync(Timeout);
        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task Each_job_gets_its_own_scope()
    {
        var seen = new List<ScopedCounter>();
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await Queue.EnqueueAsync("first", (sp, _) => { seen.Add(sp.GetRequiredService<ScopedCounter>()); return Task.CompletedTask; });
        await Queue.EnqueueAsync("second", (sp, _) => { seen.Add(sp.GetRequiredService<ScopedCounter>()); done.SetResult(); return Task.CompletedTask; });

        await done.Task.WaitAsync(Timeout);
        Assert.Equal(2, seen.Count);
        Assert.NotSame(seen[0], seen[1]);
    }

    [Fact]
    public async Task A_failing_job_is_retried_until_it_succeeds()
    {
        var attempts = 0;
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await Queue.EnqueueAsync("flaky", (_, _) =>
        {
            if (++attempts < 3)
            {
                throw new InvalidOperationException("transient");
            }

            done.SetResult();
            return Task.CompletedTask;
        });

        await done.Task.WaitAsync(Timeout);
        Assert.Equal(3, attempts);
    }

    [Fact]
    public async Task A_job_that_keeps_failing_does_not_stop_the_queue()
    {
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        await Queue.EnqueueAsync("broken", (_, _) => throw new InvalidOperationException("always"));
        await Queue.EnqueueAsync("next", (_, _) => { done.SetResult(); return Task.CompletedTask; });

        await done.Task.WaitAsync(Timeout);
    }

    [Fact]
    public async Task A_mediatr_command_is_sent_in_the_background()
    {
        var recorder = _provider.GetRequiredService<Recorder>();

        await Queue.EnqueueAsync(new Ping("hello"));

        Assert.Equal("hello", await recorder.Received.Task.WaitAsync(Timeout));
    }

    [Fact]
    public void Invalid_options_are_rejected()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddChannelBackgroundJobs(options => options.MaxConcurrency = 0);
        using var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(() => provider.GetRequiredService<IOptions<BackgroundJobOptions>>().Value);
    }

    public sealed class ScopedCounter;

    public sealed class Recorder
    {
        public TaskCompletionSource<string> Received { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public sealed record Ping(string Message) : IRequest;

    public sealed class PingHandler(Recorder recorder) : IRequestHandler<Ping>
    {
        public Task Handle(Ping request, CancellationToken cancellationToken)
        {
            recorder.Received.TrySetResult(request.Message);
            return Task.CompletedTask;
        }
    }
}
