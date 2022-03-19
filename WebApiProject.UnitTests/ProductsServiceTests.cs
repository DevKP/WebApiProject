using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApiProject.Domain.Entities;
using WebApiProject.Domain.Repositories;
using WebApiProject.Web.Models.Responses;
using WebApiProject.Web.Services;
using Xunit;

namespace WebApiProject.UnitTests
{
    public class ProductsServiceTests
    {
        private readonly Mock<IProductsRepository> _productsRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ProductsService _productsService;

        public ProductsServiceTests()
        {
            _productsRepositoryMock = new Mock<IProductsRepository>();
            _mapperMock = new Mock<IMapper>();

            var loggerMock = new Mock<ILogger<IProductsService>>();
            loggerMock.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true)
                .Callback(() => loggerMock.Verify(logger => logger.IsEnabled(It.IsAny<LogLevel>())));

            _productsService = new ProductsService(_productsRepositoryMock.Object, _mapperMock.Object, loggerMock.Object);
        }

        [Fact]
        public async void GetProductById_ProductExisting_ProductResponse()
        {
            // Arrange
            var product = new Product
            {
                Id = 1,
                Name = "Test",
                IsAvailable = true,
                Price = 42.0M,
                CategoryId = 1
            };

            var productResponse = new ProductResponseModel()
            {
                Id = product.Id,
                Name = product.Name,
                IsAvailable = product.IsAvailable,
                Price = product.Price,
                CategoryName = "Test category"
            };

            SetupGetByIdAsyncResult(product);
            _mapperMock.Setup(mapper => mapper.Map<ProductResponseModel>(It.Is<Product>(p => p == product)))
                .Returns(productResponse);

            // Act
            var result = await _productsService.GetAsync(1);

            // Assert
            result.Should().BeAssignableTo<Response<ProductResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Ok);
            result.ErrorMessage.Should().BeNull();
            result.Data.Should().Be(productResponse);

            _mapperMock.Verify(mapper => mapper.Map<ProductResponseModel>(It.IsAny<Product>()), Times.Exactly(1));
            _productsRepositoryMock.Verify(s => s.GetByIdAsync(It.IsAny<int>()), Times.Once);

            _mapperMock.VerifyNoOtherCalls();
            _productsRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void GetProductById_RepositoryReturnsNull_NotFoundResponseModel()
        {
            // Arrange
            SetupGetByIdAsyncResult(null);
            _mapperMock.Setup(mapper => mapper.Map<ProductResponseModel>(It.Is<Product>(p => p == null)))
                .Returns<ProductResponseModel>(null);

            // Act
            var result = await _productsService.GetAsync(1);

            // Assert
            result.Should().BeAssignableTo<Response<ProductResponseModel>>();
            result.Status.Should().Be(ResponseStatus.NotFound);
            result.ErrorMessage.Should().Be(nameof(ResponseStatus.NotFound));
            result.Data.Should().BeNull();

            _mapperMock.Verify(mapper => mapper.Map<ProductResponseModel>(It.IsAny<Product>()), Times.Exactly(1));
            _productsRepositoryMock.Verify(s => s.GetByIdAsync(It.IsAny<int>()), Times.Once);

            _mapperMock.VerifyNoOtherCalls();
            _productsRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void GetProductById_UnknownExceptionInService_ErrorResponseModel()
        {
            // Arrange
            _productsRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            var result = await _productsService.GetAsync(1);

            // Assert
            result.Should().BeAssignableTo<Response<ProductResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Error);
            result.ErrorMessage.Should().Be("Exception of type 'System.Exception' was thrown.");
            result.Data.Should().BeNull();

            _mapperMock.Verify(mapper => mapper.Map<ProductResponseModel>(It.IsAny<Product>()), Times.Never);
            _productsRepositoryMock.Verify(s => s.GetByIdAsync(It.IsAny<int>()), Times.Once);

            _mapperMock.VerifyNoOtherCalls();
            _productsRepositoryMock.VerifyNoOtherCalls();
        }

        private void SetupGetByIdAsyncResult(Product product)
        {
            _productsRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
        }
    }
}
