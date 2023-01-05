using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Controllers;

[Route("/api/error")]
public class ErrorController : ControllerBase
{
    [Authorize]
    [HttpPut("")]
    public IActionResult Throw()
    {
        throw new NotImplementedException("Not implemented on purpose");
    }    
}