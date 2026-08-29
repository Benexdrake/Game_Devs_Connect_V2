namespace GameDevsConnect.Api.Shared;

public enum ResultStatus
{
    Success,
    NotFound,
    Conflict,
    Forbidden,
    Unauthorized,
    ValidationError,
}

public sealed class Result<T>
{
    public ResultStatus Status { get; }
    public T? Value { get; }
    public string? Error { get; }

    private Result(ResultStatus status, T? value, string? error)
    {
        Status = status;
        Value = value;
        Error = error;
    }

    public bool IsSuccess => Status == ResultStatus.Success;

    public static Result<T> Success(T value) => new(ResultStatus.Success, value, null);
    public static Result<T> NotFound(string? error = null) => new(ResultStatus.NotFound, default, error);
    public static Result<T> Conflict(string? error = null) => new(ResultStatus.Conflict, default, error);
    public static Result<T> Forbidden(string? error = null) => new(ResultStatus.Forbidden, default, error);
    public static Result<T> Unauthorized(string? error = null) => new(ResultStatus.Unauthorized, default, error);
    public static Result<T> ValidationError(string error) => new(ResultStatus.ValidationError, default, error);
}
