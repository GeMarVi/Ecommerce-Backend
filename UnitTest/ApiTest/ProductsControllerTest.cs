using Ecommerce.Api.Controllers;
using Ecommerce.Services.IServices;
using Ecommerce.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace UnitTest.ApiTest
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductServices> _mockProductServices;
        private readonly ProductsController _controller;
        private readonly int page = 1;
        private readonly int pageSize = 12;
        private readonly string name = "fake";
        private readonly ListCompleteProductResponseDto expectedProducts = new ListCompleteProductResponseDto
        {
            completeProducts = new List<CompleteProductResponseDto>
                {

                    new CompleteProductResponseDto
                    {
                        Product_Id = 1,
                        ProductName = "Fake Product Name",
                        Description = "Fake product description",
                        Brand = "Fake Brand",
                        Model = "Fake Model",
                        ClosureType = "Fake Closure",
                        OuterMaterial = "Fake Material",
                        SoleMaterial = "Fake Sole Material",
                        TypeDeport = "Fake Sport",
                        Gender = "Unisex",
                        Color = "Red",
                        Price = 99.99m,
                        hasDiscount = true,
                        ProductStatus = "Available",
                        DiscountRate = 10.0m,
                        image = "https://fakeurl.com/fakeimage.jpg",
                        SizeStocksDto = new List<SizeStockResponseDto>
                        {
                            new SizeStockResponseDto { Size = 42.5, Stock = 10 },
                            new SizeStockResponseDto { Size = 43.0, Stock = 5 }
                        }
                    }
                },
            numberPages = 2
        };
        private readonly List<CompleteProductResponseDto> listExpectedProduct = new List<CompleteProductResponseDto>
        {
             new CompleteProductResponseDto
             {
                 Product_Id = 1,
                 ProductName = "Fake Product Name",
                 Description = "Fake product description",
                 Brand = "Fake Brand",
                 Model = "Fake Model",
                 ClosureType = "Fake Closure",
                 OuterMaterial = "Fake Material",
                 SoleMaterial = "Fake Sole Material",
                 TypeDeport = "Fake Sport",
                 Gender = "Unisex",
                 Color = "Red",
                 Price = 99.99m,
                 hasDiscount = true,
                 ProductStatus = "Available",
                 DiscountRate = 10.0m,
                 image = "https://fakeurl.com/fakeimage.jpg",
                 SizeStocksDto = new List<SizeStockResponseDto>
                 {
                     new SizeStockResponseDto { Size = 42.5, Stock = 10 },
                     new SizeStockResponseDto { Size = 43.0, Stock = 5 }
                 }
             }
        };

        public ProductControllerTests()
        {
            _mockProductServices = new Mock<IProductServices>();
            _controller = new ProductsController(_mockProductServices.Object);
        }


        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithProducts()
        {
            // Arrange
            _mockProductServices.Setup(service => service.GetProductsServices(page, pageSize))
                                .ReturnsAsync((expectedProducts));

            // Act
            var result = await _controller.GetProducts(page);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
            Assert.IsType<ListCompleteProductResponseDto>(apiResponse.Data);
            var responseData = Assert.IsType<ListCompleteProductResponseDto>(apiResponse.Data);
            Assert.Equal(expectedProducts.numberPages, responseData.numberPages);
            Assert.Equal(expectedProducts.completeProducts.Count, responseData.completeProducts.Count);
        }

        [Fact]
        public async Task GetProducts_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockProductServices.Setup(service => service.GetProductsServices(It.IsAny<int>(), It.IsAny<int>()))
                                .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetProducts(page);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse>(statusCodeResult.Value);
            Assert.False(apiResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, apiResponse.StatusCode);
        }

        [Fact]
        public async Task GetProductsByName_ReturnOk()
        {
            // Arrange
            string name = "fake";
            _mockProductServices.Setup(service => service.GetProductsByNameServices(name))
                .ReturnsAsync(listExpectedProduct);
            // Act
            var result = await _controller.GetProductByName(name);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);

            var responseData = Assert.IsType<List<CompleteProductResponseDto>>(apiResponse.Data);
        }

        [Fact]
        public async Task GetProductsByName_InternalServerError()
        {
            // Arrange
            _mockProductServices.Setup(service => service.GetProductsByNameServices(name))
                                .ThrowsAsync(new Exception());
            // Act
            var result = await _controller.GetProductByName(name);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse>(statusCodeResult.Value);
            Assert.False(apiResponse.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, apiResponse.StatusCode);
        }
    }
}
