using System.Diagnostics.CodeAnalysis;

namespace Defence.In.Depth.Domain.Services;

public enum ResultKind
{
    None = 0,
        
    Success,
        
    NotFound,
        
    NoAccessToData,
        
    NoAccessToOperation
}

public record ServiceResult<T>
{
    public T? Value { get; private init; }
    
    public ResultKind Result { get; private init; }
    
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess => Result == ResultKind.Success;

    public static ServiceResult<T> Success(T value) => new() { Result = ResultKind.Success, Value = value };
    
    public static ServiceResult<T> NotFound => new() { Result = ResultKind.NotFound };
    
    public static ServiceResult<T> NoAccessToData => new() { Result = ResultKind.NoAccessToData };
    
    public static ServiceResult<T> NoAccessToOperation => new() { Result = ResultKind.NoAccessToOperation };
}