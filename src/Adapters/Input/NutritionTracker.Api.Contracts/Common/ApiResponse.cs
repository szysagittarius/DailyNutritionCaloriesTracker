namespace NutritionTracker.Api.Contracts.Common;

/// <summary>
/// Standard API response wrapper
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResponse<T> SuccessResult(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> FailureResult(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }

    public static ApiResponse<T> FailureResult(string message, string error)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = new List<string> { error }
        };
    }
}

/// <summary>
/// API response without data payload
/// </summary>
public class ApiResponse : ApiResponse<object>
{
    public new static ApiResponse Success(string? message = null)
    {
        return new ApiResponse
        {
            Data = null,
            Message = message
        };
    }

    public static ApiResponse Failure(string message, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Data = null,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}
