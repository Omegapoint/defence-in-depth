namespace Defence.In.Depth.Domain.Services;

public enum ReadDataResult
{
    None = 0,
        
    Success,
        
    NotFound,
        
    NoAccessToData,
        
    NoAccessToOperation
}