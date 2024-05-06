[![Nuget](https://img.shields.io/nuget/v/Mehedi.Core.SharedKernel)](https://www.nuget.org/packages/Mehedi.Core.SharedKernel/)
[![Nuget](https://img.shields.io/nuget/dt/Mehedi.Core.SharedKernel)](https://www.nuget.org/packages/Mehedi.Core.SharedKernel/)

# Mehedi.Application.SharedKernel
Some useful base classes and interfaces, mainly used with the Application layer inside CleanArchitecture template. Also, a template to make your own Application layer SharedKernel nuget package.

## Technologies
- .NET Core 8
- C#
- MediatR
- xUnit

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
