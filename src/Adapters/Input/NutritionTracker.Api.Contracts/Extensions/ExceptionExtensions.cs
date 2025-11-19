using NutritionTracker.Api.Contracts.Common;
using NutritionTracker.Api.Contracts.Exceptions;

namespace NutritionTracker.Api.Contracts.Extensions;

/// <summary>
/// Extension methods for exception handling
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Converts an exception to an ErrorResponse
    /// </summary>
    public static ErrorResponse ToErrorResponse(this Exception exception, bool includeStackTrace = false)
    {
        return exception switch
        {
            ValidationException valEx => new ErrorResponse(valEx.Message, valEx.ValidationErrors, valEx.ErrorCode),
            ApiException apiEx => new ErrorResponse(apiEx.Message, apiEx.ErrorCode),
            InvalidOperationException => new ErrorResponse(exception.Message, "INVALID_OPERATION"),
            ArgumentException => new ErrorResponse(exception.Message, "INVALID_ARGUMENT"),
            _ => new ErrorResponse("An unexpected error occurred", "INTERNAL_ERROR")
            {
                StackTrace = includeStackTrace ? exception.StackTrace : null
            }
        };
    }

    /// <summary>
    /// Gets the appropriate HTTP status code for an exception
    /// </summary>
    public static int GetStatusCode(this Exception exception)
    {
        return exception switch
        {
            ValidationException => 400,
            NotFoundException => 404,
            UnauthorizedException => 401,
            ForbiddenException => 403,
            ApiException apiEx => apiEx.StatusCode,
            InvalidOperationException => 400,
            ArgumentException => 400,
            _ => 500
        };
    }
}
