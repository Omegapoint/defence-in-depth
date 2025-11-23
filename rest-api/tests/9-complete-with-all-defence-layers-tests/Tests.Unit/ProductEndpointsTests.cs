using System.Security.Claims;
using CompleteWithAllDefenceLayers.Tests.Unit.Mock;
using Defence.In.Depth.DataContracts;
using Defence.In.Depth.Domain.Services;
using Defence.In.Depth.Endpoints;
using Defence.In.Depth.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductEndpointsTests
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
        
        var result = await ProductEndpoints.GetById("se1", productService);

        Assert.IsType<Ok<ProductDataContract>>(result);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturnDataContract_WhenAuthorized()
    {
        var productService = CreateSutWithAllAccess();

        var result = await ProductEndpoints.GetById("se1", productService);

        var valueHttpResult = Assert.IsType<IValueHttpResult>(result, exactMatch: false);
        Assert.IsType<IDataContract>(valueHttpResult.Value, exactMatch: false);
    }

    [Theory]
    [MemberData(nameof(InvalidIds))]
    public async Task GetProductsById_ShouldReturn400_WhenInvalidId(string id)
    {
        var productService = CreateSutWithAllAccess();

        var result = await ProductEndpoints.GetById(id, productService);

        var badRequest = Assert.IsType<BadRequest<string>>(result);
        Assert.Equal("Id is not valid.", badRequest.Value);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturn404_WhenNotFound()
    {
        var productService = CreateSutWithAllAccess();

        var result = await ProductEndpoints.GetById("def", productService); // This is a valid, non-existing id

        Assert.IsType<NotFound>(result);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturn403_WhenCanNotRead()
    {
        var productService = CreateSutWithNoReadAccess();

        var result = await ProductEndpoints.GetById("se1", productService);

        Assert.IsType<ForbidHttpResult>(result);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturn404_WhenNoAccessToData()
    {
        var productService = CreateSutWithAllAccess();

        // The user should only be able to access products on the SE-market
        var result = await ProductEndpoints.GetById("no1", productService); 

        Assert.IsType<NotFound>(result);
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


