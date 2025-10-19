namespace ProductVariantBundle.Api.DTOs.Common;

public class ApiResponse<T>
{
    public T? Data { get; set; }
    public object? Meta { get; set; }
    public IEnumerable<string> Errors { get; set; } = new List<string>();

    public static ApiResponse<T> Success(T data, object? meta = null)
    {
        return new ApiResponse<T>
        {
            Data = data,
            Meta = meta,
            Errors = new List<string>()
        };
    }

    public static ApiResponse<T> Error(IEnumerable<string> errors)
    {
        return new ApiResponse<T>
        {
            Data = default,
            Meta = null,
            Errors = errors
        };
    }

    public static ApiResponse<T> Error(string error)
    {
        return Error(new[] { error });
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Success(object? meta = null)
    {
        return new ApiResponse
        {
            Data = null,
            Meta = meta,
            Errors = new List<string>()
        };
    }

    public new static ApiResponse Error(IEnumerable<string> errors)
    {
        return new ApiResponse
        {
            Data = null,
            Meta = null,
            Errors = errors
        };
    }

    public new static ApiResponse Error(string error)
    {
        return Error(new[] { error });
    }
}