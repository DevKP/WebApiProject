using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApiProject.Domain.Entities;
using WebApiProject.Domain.Repositories;
using WebApiProject.Web.Data;
using WebApiProject.Web.Models.Responses;
using WebApiProject.Web.Services;
using Xunit;

namespace WebApiProject.UnitTests
{
    public class ProductsServiceTests
    {
        private readonly Mock<IProductsRepository> _productsRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<IProductsService>> _loggerMock;
        private readonly ProductsService _productsService;
        private readonly Fixture _fixture = new();

        public ProductsServiceTests()
        {
            ConfigureFixtureBehavior();

            _productsRepositoryMock = new Mock<IProductsRepository>();
            _mapperMock = new Mock<IMapper>();

            _loggerMock = new Mock<ILogger<IProductsService>>();
            _loggerMock.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true)
                .Callback(() => _loggerMock.Verify(logger => logger.IsEnabled(It.IsAny<LogLevel>())));

            _productsService = new ProductsService(_productsRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void Ctor_WhenProductsRepositoryIsNull_ThenThrowArgumentNullException()
        {
            Action action = () => new ProductsService(null, _mapperMock.Object, _loggerMock.Object);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_WhenMapperIsNull_ThenThrowArgumentNullException()
        {
            Action action = () => new ProductsService(_productsRepositoryMock.Object, null, _loggerMock.Object);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_WhenLoggerIsNull_ThenThrowArgumentNullException()
        {
            Action action = () => new ProductsService(_productsRepositoryMock.Object, _mapperMock.Object, null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async void GetProductById_WhenProductExisting_ThenProductResponse()
        {
            // Arrange
            var product = _fixture.Create<Product>();

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
        public async void GetProductById_WhenRepositoryReturnsNull_ThenNotFoundResponseModel()
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
            result.ErrorMessage.Should().Be(ErrorMessages.NotFoundInDatabase);
            result.Data.Should().BeNull();

            _mapperMock.Verify(mapper => mapper.Map<ProductResponseModel>(It.IsAny<Product>()), Times.Exactly(1));
            _productsRepositoryMock.Verify(s => s.GetByIdAsync(It.IsAny<int>()), Times.Once);

            _mapperMock.VerifyNoOtherCalls();
            _productsRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void GetProductById_WhenUnknownExceptionInService_ThenErrorResponseModel()
        {
            // Arrange
            _productsRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            var result = await _productsService.GetAsync(1);

            // Assert
            result.Should().BeAssignableTo<Response<ProductResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Error);
            result.ErrorMessage.Should().Be(ErrorMessages.ErrorWhileRetrievingEntity);
            result.Data.Should().BeNull();

            _mapperMock.Verify(mapper => mapper.Map<ProductResponseModel>(It.IsAny<Product>()), Times.Never);
            _productsRepositoryMock.Verify(s => s.GetByIdAsync(It.IsAny<int>()), Times.Once);

            _mapperMock.VerifyNoOtherCalls();
            _productsRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void GetAll_WhenProductsTableNotEmpty_ThenProductListResponse()
        {
            // Arrange
            var products = _fixture.CreateMany<Product>().ToList();
            var productListResponse = new ProductsListResponseModel()
            {
                Products = products.Select(product => new ProductResponseModel()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    IsAvailable = product.IsAvailable,
                    CategoryName = "Test Category1"
                }).ToList()
            };

            _productsRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);
            _mapperMock.Setup(mapper => mapper.Map<ProductsListResponseModel>(It.IsAny<IEnumerable<Product>>()))
                .Returns(productListResponse);

            // Act
            var result = await _productsService.GetAllAsync();

            // Assert
            result.Should().BeAssignableTo<Response<ProductsListResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Ok);
            result.ErrorMessage.Should().BeNull();
            result.Data.Should().Be(productListResponse);

            _mapperMock.Verify(mapper => mapper.Map<ProductsListResponseModel>
                (It.IsAny<IEnumerable<Product>>()), Times.Once);
            _productsRepositoryMock.Verify(s => s.GetAllAsync(), Times.Once);

            _mapperMock.VerifyNoOtherCalls();
            _productsRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void GetAll_WhenProductsTableEmpty_ThenEmptyListInResponse()
        {
            // Arrange
            var productListResponse = new ProductsListResponseModel()
            {
                Products = new List<ProductResponseModel>()
            };

            _productsRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Product>());
            _mapperMock.Setup(mapper => mapper.Map<ProductsListResponseModel>(It.IsAny<IEnumerable<Product>>()))
                .Returns(productListResponse);

            // Act
            var result = await _productsService.GetAllAsync();

            // Assert
            result.Should().BeAssignableTo<Response<ProductsListResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Ok);
            result.ErrorMessage.Should().BeNull();
            result.Data.Should().Be(productListResponse);
            result.Data.Products.Should().BeEmpty();

            _mapperMock.Verify(mapper => mapper.Map<ProductsListResponseModel>
                (It.IsAny<IEnumerable<Product>>()), Times.Once);
            _productsRepositoryMock.Verify(s => s.GetAllAsync(), Times.Once);

            _mapperMock.VerifyNoOtherCalls();
            _productsRepositoryMock.VerifyNoOtherCalls();
        }

        private void SetupGetByIdAsyncResult(Product product)
        {
            _productsRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
        }

        private void ConfigureFixtureBehavior()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}
