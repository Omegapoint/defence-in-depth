namespace Defence.In.Depth.Endpoints;

public static class ErrorEndpoints
{
    public static void RegisterErrorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/error", _ => throw  new NotImplementedException());
    }
}