namespace NutritionTracker.Api.Contracts.Common;

/// <summary>
/// Standard error response
/// </summary>
public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public List<string> Details { get; set; } = new();
    public string? StackTrace { get; set; }

    public ErrorResponse() { }

    public ErrorResponse(string message, string? errorCode = null)
    {
        Message = message;
        ErrorCode = errorCode;
    }

    public ErrorResponse(string message, List<string> details, string? errorCode = null)
    {
        Message = message;
        Details = details;
        ErrorCode = errorCode;
    }
}
