[![Nuget](https://img.shields.io/nuget/v/Mehedi.Application.SharedKernel)](https://www.nuget.org/packages/Mehedi.Application.SharedKernel/)
[![Nuget](https://img.shields.io/nuget/dt/Mehedi.Application.SharedKernel)](https://www.nuget.org/packages/Mehedi.Application.SharedKernel/)

# Mehedi.Application.SharedKernel
Some useful base classes and interfaces, mainly used with the Application layer inside CleanArchitecture template. Also, a template to make your own Application layer SharedKernel nuget package.

## Background jobs without Hangfire

`AddChannelBackgroundJobs` adds an in-process job queue built on `System.Threading.Channels` and a hosted
`BackgroundService`. It needs no database, server or dashboard.

```csharp
builder.Services.AddChannelBackgroundJobs(options =>
{
    options.Capacity = 1000;        // EnqueueAsync waits when the queue is full (back-pressure)
    options.MaxConcurrency = 1;     // jobs run in order; raise it for parallel work
    options.MaxAttempts = 3;        // retries with a growing delay
    options.RetryDelay = TimeSpan.FromSeconds(2);
});
```

Queue a MediatR command and answer the HTTP request at once:

```csharp
[HttpPost("async")]
public async Task<IActionResult> CreateLater(CreateCustomerCommand command, IBackgroundJobQueue jobs, CancellationToken ct)
{
    var jobId = await jobs.EnqueueAsync(command, ct);   // runs later in a new DI scope
    return Accepted(new { jobId });
}
```

Or queue any delegate:

```csharp
await jobs.EnqueueAsync("send-welcome-email", async (services, ct) =>
{
    var mailer = services.GetRequiredService<IMailer>();
    await mailer.SendWelcomeAsync(customerId, ct);
});
```

| | Channel queue | Hangfire |
|---|---|---|
| Storage | memory | SQL/Redis/PostgreSQL |
| Survives a restart | **no**, queued jobs are lost (and logged) | yes |
| Dashboard, scheduling, cron | no | yes |
| Infrastructure | none | a database and a server |
| Latency | microseconds | polling interval |

Use the channel queue for work that is fast and can be repeated or rebuilt. Work that must never be lost
should go through a transactional outbox (see `Mehedi.Write.RDBMS.Infrastructure.Abstractions`) or a durable broker.

## Technologies
- .NET Core 8
- C#
- MediatR
- xUnit

## Prerequisites
- MediatR (>= 12.5.0)
- Microsoft.Extensions.Configuration.Abstractions (>= 8.0.0)
- Microsoft.Extensions.Logging (>= 8.0.0)
- Microsoft.Extensions.Hosting.Abstractions (>= 8.0.1)
- Microsoft.Extensions.Options (>= 8.0.2)
- Newtonsoft.Json (>= 13.0.3)
- Mehedi.Core.SharedKernel (>= 1.0.0)

## Packaging
To pack nuget package write the following command
```
dotnet pack
```

To publish package of Mehedi.Application.SharedKernel.1.0.0.nupkg write the following command
```
dotnet nuget push .\bin\Release\Mehedi.Application.SharedKernel.1.0.0.nupkg --api-key <api-key> --source https://api.nuget.org/v3/index.json
```

## References
- [Ardalis.SharedKernel](https://github.com/ardalis/Ardalis.SharedKernel)
- [Clean Architecture Solution Template](https://github.com/jasontaylordev/CleanArchitecture)
- [ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, SOLID Principles and Clean Architecture](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing)
