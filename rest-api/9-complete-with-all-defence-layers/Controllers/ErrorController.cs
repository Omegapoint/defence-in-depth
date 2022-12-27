using Microsoft.AspNetCore.Mvc;

namespace Defence.In.Depth.Controllers;

[Route("/api/error")]
public class ErrorController : ControllerBase
{
    [HttpPut]
    [Route("")]
    public IActionResult Throw()
    {
        throw new NotImplementedException("Not implemented on purpose");
    }    
}