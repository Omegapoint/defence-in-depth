using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Defence.In.Depth.Endpoints;

public static class HealthEndpoints
{
    public static void RegisterHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/health/live", Live)
            .AllowAnonymous();

        app.MapGet("/api/health/ready", Ready)
            .RequireAuthorization();
    }

    
    private static IResult Live()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        return Results.Ok(
            new
            {
                Version = $"{version?.Major}.{version?.Minor}.{version?.Build}",
                Details = versionInfo.ProductVersion
            });
    }

    private static IResult Ready()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        // This is where we might make additional checks for the readiness of the
        // application, for example if dependencies are ready for requests.        

        return Results.Ok(
            new
            {
                Version = $"{version?.Major}.{version?.Minor}.{version?.Build}",
                Details = versionInfo.ProductVersion,

                // For an authorized call, we can reveal more details, like the runtime version
                Runtime = $"{RuntimeInformation.FrameworkDescription}@{RuntimeInformation.RuntimeIdentifier}"
            });
    }
}