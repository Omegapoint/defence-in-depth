using AutoMapper;
using Defence.In.Depth;
using Defence.In.Depth.Controllers;
using Defence.In.Depth.DataContracts;
using Defence.In.Depth.Domain.Models;
using Defence.In.Depth.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductsControllerTests
{
    private readonly IMapper mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();

    [Fact]
    public async Task GetProductsById_ShouldReturn200_WhenAuthorized()
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService).Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .ReturnsAsync((
                    new Product(
                        new ProductId("se1"), 
                        new ProductName("ProductSweden"), 
                        new MarketId("se")),
                    ReadDataResult.Success));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById("se1");

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturnDataContract_WhenAuthorized()
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService).Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .ReturnsAsync((
                    new Product(
                        new ProductId("se1"), 
                        new ProductName("ProductSweden"), 
                        new MarketId("se")),
                    ReadDataResult.Success));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById("se1");

        Assert.IsAssignableFrom<IDataContract>((result.Result as ObjectResult)?.Value);
    }

    [Theory]
    [MemberData(nameof(InvalidIds))]
    public async Task GetProductsById_ShouldReturn400_WhenInvalidId(string id)
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService).Setup(ps => ps.GetById(It.IsAny<ProductId>()))
             .ReturnsAsync((null, ReadDataResult.NoAccessToData));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById(id);

        Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturn404_WhenNotFound()
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService).Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .ReturnsAsync((null, ReadDataResult.NoAccessToData));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById("def"); // This is a valid, non-existing id

        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturn403_WhenCanNotRead()
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService).Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .ReturnsAsync((null, ReadDataResult.NoAccessToOperation));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById("se1");

        Assert.IsType<ForbidResult>(result.Result);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetProductsById_ShouldReturn404_WhenNoAccessToData()
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService).Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .ReturnsAsync((null, ReadDataResult.NoAccessToData));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById("no1"); // The user should only be aable to access products on teh SE-market

        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Null(result.Value);
    }

    //Some examples of invalid id:s, this is verified in more depth in e g the PriductId unit tests
    public static IEnumerable<object[]> InvalidIds => new[]
    {
            new object[] { "" },
            new object[] { "no spaces" },
            new object[] { "thisisanidthatistoolong" },
            new object[] { "#" },
            new object[] { "<script>" }
        };
}


