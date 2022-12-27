using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Controllers;

[Route("/api/health")]
public class HealthController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("live")]
    public ActionResult Live()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        return Ok(
            new
            {
                Version = $"{version?.Major}.{version?.Minor}.{version?.Build}",
                Details = versionInfo.ProductVersion
            });
    }

    [Authorize]
    [HttpGet("ready")]
    public ActionResult Ready()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        // This is where we might make additional checks for the readiness of the
        // application, for example if dependencies are ready for requests.        

        return Ok(
            new
            {
                Version = $"{version?.Major}.{version?.Minor}.{version?.Build}",
                Details = versionInfo.ProductVersion,

                // For an authorized call, we can reveal more details, like the runtime version
                Runtime = $"{RuntimeInformation.FrameworkDescription}@{RuntimeInformation.RuntimeIdentifier}"
            });
    }
}