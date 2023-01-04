using AutoMapper;
using Defence.In.Depth;
using Defence.In.Depth.Controllers;
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
    public async void GetProductsById_ShouldReturn403_WhenCanNotRead()
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService).Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .ReturnsAsync((
                new Product(
                    new ProductId("se1"),
                    new ProductName("ProductSweden"),
                    new MarketId("se")),
                ReadDataResult.NoAccessToOperation));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById("se1");

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async void GetProductsById_ShouldReturn200_WhenAuthorized()
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

    [Theory]
    [MemberData(nameof(IdInjection))]
    [MemberData(nameof(InvalidIds))]
    public async void GetProductsById_ShouldReturn400_WhenInvalidId(string id)
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

        var result = await controller.GetById(id);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async void GetProductsById_ShouldReturn404_WhenNotFound()
    {
        var productService = Mock.Of<IProductService>();
        Mock.Get(productService).Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .ReturnsAsync((
                new Product(
                    new ProductId("se1"),
                    new ProductName("ProductSweden"),
                    new MarketId("se")),
                ReadDataResult.NotFound));

        var controller = new ProductsController(productService, mapper);

        var result = await controller.GetById("def"); // This is a valid, non-existing id

        Assert.IsType<NotFoundResult>(result.Result);
    }

    public static IEnumerable<object[]> IdInjection => new[]
    {
            new object[] { "<script>" },
            new object[] { "'1==1" },
            new object[] { "--sql" }
        };

    public static IEnumerable<object[]> InvalidIds => new[]
    {
            new object[] { "" },
            new object[] { "no spaces" },
            new object[] { "thisisanidthatistoolong" },
            new object[] { "#" }
        };
}


