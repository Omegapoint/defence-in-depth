using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace CompleteWithAllDefenceLayers.Tests.Unit.Mock;

public class HttpContextAccessorMock : IHttpContextAccessor
{
    private readonly HttpContext testContext;

    public HttpContextAccessorMock(ClaimsIdentity identity)
    {
        testContext = new DefaultHttpContext();
        testContext.User.AddIdentity(identity);
    }
    public HttpContext? HttpContext { get => testContext; set => throw new NotImplementedException(); }
}
