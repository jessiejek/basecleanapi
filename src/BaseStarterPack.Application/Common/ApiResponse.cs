namespace BaseStarterPack.Application.Common;

public class ApiResponse<T>
{
    public T? Data { get; private set; }
    public string? Message { get; private set; }
    public bool IsSuccess { get; private set; }

    private ApiResponse(bool isSuccess, T? data = default, string? message = null)
    {
        IsSuccess = isSuccess;
        Data = data;
        Message = message;
    }

    public static ApiResponse<T> Success(T data, string? message = null)
        => new(true, data, message);

    public static ApiResponse<T> Fail(string message)
        => new(false, default, message);

    public static ApiResponse<T> EmptySuccess(string? message = null)
        => new(true, default, message);
}
