namespace NutritionTracker.Api.Contracts.Exceptions;

/// <summary>
/// Base exception for API-specific errors
/// </summary>
public abstract class ApiException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }

    protected ApiException(string message, string errorCode, int statusCode)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }

    protected ApiException(string message, string errorCode, int statusCode, Exception innerException)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        StatusCode = statusCode;
    }
}

/// <summary>
/// Exception for validation errors (400 Bad Request)
/// </summary>
public class ValidationException : ApiException
{
    public List<string> ValidationErrors { get; }

    public ValidationException(string message, List<string> validationErrors)
        : base(message, "VALIDATION_ERROR", 400)
    {
        ValidationErrors = validationErrors;
    }

    public ValidationException(string message, string validationError)
        : base(message, "VALIDATION_ERROR", 400)
    {
        ValidationErrors = new List<string> { validationError };
    }
}

/// <summary>
/// Exception for resource not found (404 Not Found)
/// </summary>
public class NotFoundException : ApiException
{
    public NotFoundException(string resourceName, object key)
        : base($"{resourceName} with identifier '{key}' was not found.", "RESOURCE_NOT_FOUND", 404)
    {
    }

    public NotFoundException(string message)
        : base(message, "RESOURCE_NOT_FOUND", 404)
    {
    }
}

/// <summary>
/// Exception for business rule violations (409 Conflict)
/// </summary>
public class BusinessRuleException : ApiException
{
    public BusinessRuleException(string message)
        : base(message, "BUSINESS_RULE_VIOLATION", 409)
    {
    }

    public BusinessRuleException(string message, Exception innerException)
        : base(message, "BUSINESS_RULE_VIOLATION", 409, innerException)
    {
    }
}

/// <summary>
/// Exception for unauthorized access (401 Unauthorized)
/// </summary>
public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message = "Unauthorized access")
        : base(message, "UNAUTHORIZED", 401)
    {
    }
}

/// <summary>
/// Exception for forbidden access (403 Forbidden)
/// </summary>
public class ForbiddenException : ApiException
{
    public ForbiddenException(string message = "Access forbidden")
        : base(message, "FORBIDDEN", 403)
    {
    }
}
