namespace Mehedi.Application.SharedKernel.Enums;

/// <summary>
/// ResultStatus enum, will be useful during http responses
/// </summary>
public enum ResultStatus
{
    Ok,
    Error,
    Forbidden,
    Unauthorized,
    Invalid,
    NotFound,
    Conflict,
    CriticalError,
    Unavailable
}
