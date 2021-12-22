using Defence.In.Depth.Domain.Model;
using Defence.In.Depth.Domain.Services;
using Xunit;
using System.Threading.Tasks;
using Moq;
using System.Collections.Generic;
using Defence.In.Depth.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CompleteWithAllDefenceLayers.Test;

[Trait("Category", "Unit")]
public class ProductsControllerTests
{
    [Fact]
    public async void GetProductsByIdShouldReturn403WhenCanNotRead()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(ps => ps.GetById(It.IsAny<ProductId>())).Returns(Task.FromResult((new Product(new ProductId("productSE"), new ProductName("productSE"), new MarketId("se")), ReadDataResult.NoAccessToOperation)));

        var controller = new ProductsController(productServiceMock.Object, TestMapper.Create());

        var result = await controller.GetById("productSE");

        Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public async void GetProductsByIdShouldReturn200WhenAuthorized()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(ps => ps.GetById(It.IsAny<ProductId>()))
            .Returns(Task.FromResult((new Product(new ProductId("productSE"), new ProductName("productSE"), new MarketId("se")), ReadDataResult.Success)));

        var controller = new ProductsController(productServiceMock.Object, TestMapper.Create());

        var result = await controller.GetById("productSE");

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Theory]
    [MemberData(nameof(IdInjection))]
    [MemberData(nameof(InvalidIds))]
    public async void GetProductsByIdShouldReturn400WhenInvalidId(string id)
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(ps => ps.GetById(It.IsAny<ProductId>()))
             .Returns(Task.FromResult((new Product(new ProductId("productSE"), new ProductName("productSE"), new MarketId("se")), ReadDataResult.Success)));

        var controller = new ProductsController(productServiceMock.Object, TestMapper.Create());

        var result = await controller.GetById(id);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async void GetProductsByIdShouldReturn404WhenNotFound()
    {
        var productServiceMock = new Mock<IProductService>();
        productServiceMock.Setup(ps => ps.GetById(It.IsAny<ProductId>())).Returns(Task.FromResult((new Product(new ProductId("productSE"), new ProductName("productSE"), new MarketId("se")), ReadDataResult.NotFound)));

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


