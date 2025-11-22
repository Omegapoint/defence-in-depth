using System.Security.Claims;
using CompleteWithAllDefenceLayers.Tests.Unit.Mock;
using Defence.In.Depth.Controllers;
using Defence.In.Depth.DataContracts;
using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductsControllerTests
{
    //Some examples of invalid id:s, this is verified in more depth in e g the ProductId unit tests
    public static IEnumerable<object[]> InvalidIds =>
    [
        [""],
        ["no spaces"],
        ["thisisanidthatistoolong"],
        ["#"],
        ["<script>"]
    ];
    
    [Fact]
    public async Task GetProductsById_ShouldReturn200_WhenAuthorized()
    {
        var productService = CreateSutWithAllAccess();
        
        var controller = new ProductsController(productService);

        var result = await controller.GetById("se1");

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturnDataContract_WhenAuthorized()
    {
        var productService = CreateSutWithAllAccess();

        var controller = new ProductsController(productService);

        var result = await controller.GetById("se1");

        Assert.IsAssignableFrom<IDataContract>((result.Result as ObjectResult)?.Value);
    }

    [Theory]
    [MemberData(nameof(InvalidIds))]
    public async Task GetProductsById_ShouldReturn400_WhenInvalidId(string id)
    {
        var productService = CreateSutWithAllAccess();

        var controller = new ProductsController(productService);

        var result = await controller.GetById(id);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturn404_WhenNotFound()
    {

        var productService = CreateSutWithAllAccess();

        var controller = new ProductsController(productService);

        var result = await controller.GetById("def"); // This is a valid, non-existing id

        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturn403_WhenCanNotRead()
    {
        var productService = CreateSutWithNoReadAccess();

        var controller = new ProductsController(productService);

        var result = await controller.GetById("se1");

        Assert.IsType<ForbidResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturn404_WhenNoAccessToData()
    {
        var productService = CreateSutWithAllAccess();

        var controller = new ProductsController(productService);
        
        // The user should only be able to access products on the SE-market
        var result = await controller.GetById("no1"); 

        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Null(result.Value);
    }

    private static ProductService CreateSutWithAllAccess()
    {
        var claims = new[]
        {
            new Claim(ClaimSettings.Sub, "user1"),
            new Claim(ClaimSettings.ClientId, "client1"),
            new Claim(ClaimSettings.Amr, ClaimSettings.AuthenticationMethodPassword),
            new Claim(ClaimSettings.Scope, ClaimSettings.ProductsRead),
            new Claim(ClaimSettings.Scope, ClaimSettings.ProductsWrite)
        };

        // Note that in a real-world application we usually need to mock repositories
        var productRepository = new ProductRepository();
        var mockHttpContextAccessor = new HttpContextAccessorMock(new ClaimsIdentity(claims));
        var permissionService = new HttpContextPermissionService(mockHttpContextAccessor);
        var auditService = new LoggerAuditService(new LoggerMock(), permissionService);
        
        return new ProductService(permissionService, productRepository, auditService);
    }
    
    private static ProductService CreateSutWithNoReadAccess()
    {
        var claims = new[]
        {
                new Claim(ClaimSettings.Sub, "user1"),
                new Claim(ClaimSettings.ClientId, "client1"),
                new Claim(ClaimSettings.Amr, ClaimSettings.AuthenticationMethodPassword),
                new Claim(ClaimSettings.Scope, ClaimSettings.ProductsWrite)
        };
        
        // Note that in a real-world application we usually need to mock repositories
        var productRepository = new ProductRepository();
        var mockHttpContextAccessor = new HttpContextAccessorMock(new ClaimsIdentity(claims));
        var permissionService = new HttpContextPermissionService(mockHttpContextAccessor);
        var auditService = new LoggerAuditService(new LoggerMock(), permissionService);

        return new ProductService(permissionService, productRepository, auditService);
    }
}


