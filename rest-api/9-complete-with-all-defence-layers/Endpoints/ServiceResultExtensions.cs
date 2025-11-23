using Defence.In.Depth.DataContracts;
using Defence.In.Depth.Domain.Services;

namespace Defence.In.Depth.Endpoints;

public static class ServiceResultExtensions
{
    public static IResult MapToHttpResult<T>(this ServiceResult<T> result, Func<T, IDataContract> selector)
    {
        if (result.IsSuccess)
        {
            var contract = selector(result.Value);

            return Results.Ok(contract);
        }

        return result.Result switch
        {
            ResultKind.NoAccessToOperation => Results.Forbid(),
            ResultKind.NotFound or ResultKind.NoAccessToData => Results.NotFound(),

            _ => throw new InvalidOperationException($"Result kind {result} is not supported")
        };
    }
}