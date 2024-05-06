using Mehedi.Application.SharedKernel.Enums;
using Mehedi.Application.SharedKernel.Exceptions;

namespace Mehedi.Application.SharedKernel.Responses;

/// <summary>
/// Result interface will be used on generating http responses
/// </summary>
public interface IResult
{
    /// <summary>
    /// Result Status
    /// </summary>
    ResultStatus Status { get; }
    /// <summary>
    /// List of Errors
    /// </summary>
    IEnumerable<string> Errors { get; }
    /// <summary>
    /// List of validation related errors
    /// </summary>
    IEnumerable<ValidationException> ValidationErrors { get; }
    /// <summary>
    /// returns the value type
    /// </summary>
    Type ValueType { get; }
    /// <summary>
    /// returns value as object, need to cast specific object type
    /// </summary>
    /// <returns></returns>
    object GetValue();
}
