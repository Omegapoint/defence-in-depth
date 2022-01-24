using Defence.In.Depth.Controllers;
using Defence.In.Depth.Domain.Model;
using Defence.In.Depth.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CompleteWithAllDefenceLayers.Tests.Unit;

[Trait("Category", "Unit")]
public class ProductsControllerTests
{
    [Fact]
    public async void GetProductsById_ShouldReturn403_WhenCanNotRead()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .ReturnsAsync((
                new Product(
                    new ProductId("se1"),
                    new ProductName("ProductSweden"),
                    new MarketId("se")),
                ReadDataResult.NoAccessToOperation));

        var controller = new ProductsController(productServiceMock.Object, TestMapper.Create());

        var result = await controller.GetById("se1");

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async void GetProductsById_ShouldReturn200_WhenAuthorized()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .ReturnsAsync((
                    new Product(
                        new ProductId("se1"), 
                        new ProductName("ProductSweden"), 
                        new MarketId("se")),
                    ReadDataResult.Success));

        var controller = new ProductsController(productServiceMock.Object, TestMapper.Create());

        var result = await controller.GetById("se1");

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Theory]
    [MemberData(nameof(IdInjection))]
    [MemberData(nameof(InvalidIds))]
    public async void GetProductsById_ShouldReturn400_WhenInvalidId(string id)
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(ps => ps.GetById(It.IsAny<ProductId>()))
             .ReturnsAsync((
                 new Product(
                     new ProductId("se1"),
                     new ProductName("ProductSweden"),
                     new MarketId("se")), 
                 ReadDataResult.Success));

        var controller = new ProductsController(productServiceMock.Object, TestMapper.Create());

        var result = await controller.GetById(id);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async void GetProductsById_ShouldReturn404_WhenNotFound()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .ReturnsAsync((
                new Product(
                    new ProductId("se1"),
                    new ProductName("ProductSweden"),
                    new MarketId("se")),
                ReadDataResult.NotFound));

        var controller = new ProductsController(productServiceMock.Object, TestMapper.Create());

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


