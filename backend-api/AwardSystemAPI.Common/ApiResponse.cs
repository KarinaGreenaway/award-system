namespace AwardSystemAPI.Common;

public readonly struct ApiResponse<TValue, TError>
{
    private readonly TValue? _value;
    private readonly TError? _error;

    private ApiResponse(TValue value)
    {
        IsError = false;
        _value = value;
        _error = default;
    }
    
    private ApiResponse(TError error)
    {
        IsError = true;
        _value = default;
        _error = error;
    }
    
    public bool IsError { get; }
    
    public bool IsSuccess => !IsError;

    public static implicit operator ApiResponse<TValue, TError>(TValue value) => new(value);
    
    public static implicit operator ApiResponse<TValue, TError>(TError error) => new(error);
    
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<TError, TResult> onError)
    {
        return IsError ? onError(_error!) : onSuccess(_value!);
    }

}