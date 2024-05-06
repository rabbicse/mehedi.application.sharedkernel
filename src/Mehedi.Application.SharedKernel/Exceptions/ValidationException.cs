using Mehedi.Application.SharedKernel.Enums;

namespace Mehedi.Application.SharedKernel.Exceptions;

/// <summary>
/// ValidationException will be used on validation related exception
/// </summary>
public class ValidationException
{
    public ValidationException()
    {
    }

    public ValidationException(string errorMessage) => ErrorMessage = errorMessage;

    public ValidationException(string identifier, string errorMessage, string errorCode, ValidationSeverity severity)
    {
        Identifier = identifier;
        ErrorMessage = errorMessage;
        ErrorCode = errorCode;
        Severity = severity;
    }

    public string Identifier { get; set; }
    public string ErrorMessage { get; set; }
    public string ErrorCode { get; set; }
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
}
