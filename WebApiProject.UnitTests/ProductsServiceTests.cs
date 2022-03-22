using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
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
        private readonly ProductsService sut;
        private readonly Fixture _fixture = new();

        public ProductsServiceTests()
        {
            SetFixtureRecursionDepth(1);

            _productsRepositoryMock = new Mock<IProductsRepository>();
            _mapperMock = new Mock<IMapper>();

            _loggerMock = new Mock<ILogger<IProductsService>>();
            _loggerMock.Setup(logger => logger.IsEnabled(It.IsAny<LogLevel>()))
                .Returns(true)
                .Callback(() => _loggerMock.Verify(logger => logger.IsEnabled(It.IsAny<LogLevel>())));

            sut = new ProductsService(_productsRepositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
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
        public async Task GetProductById_WhenProductExists_ThenReturnOkProductResponse()
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
            SetupMapperForGetByIdAction(productResponse);

            // Act
            var result = await sut.GetAsync(1);

            // Assert
            result.Should().BeAssignableTo<Response<ProductResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Ok);
            result.ErrorMessage.Should().BeNull();
            result.Data.Should().Be(productResponse);

            _mapperMock.Verify(mapper => mapper.Map<ProductResponseModel>(It.IsAny<Product>()), Times.Exactly(1));
            _productsRepositoryMock.Verify(s => s.GetByIdAsync(It.IsAny<int>()), Times.Once);

            VerifyNoOtherCallsOnMocks();
        }

        [Fact]
        public async Task GetProductById_WhenRepositoryReturnsNull_ThenReturnNotFoundResponse()
        {
            // Arrange
            SetupGetByIdAsyncResult(null);
            SetupMapperForGetByIdAction(null);

            // Act
            var result = await sut.GetAsync(1);

            // Assert
            result.Should().BeAssignableTo<Response<ProductResponseModel>>();
            result.Status.Should().Be(ResponseStatus.NotFound);
            result.ErrorMessage.Should().Be(ErrorMessages.NotFoundInDatabase);
            result.Data.Should().BeNull();

            _mapperMock.Verify(mapper => mapper.Map<ProductResponseModel>(It.IsAny<Product>()), Times.Never);
            _productsRepositoryMock.Verify(s => s.GetByIdAsync(It.IsAny<int>()), Times.Once);

            VerifyNoOtherCallsOnMocks();
        }

        [Fact]
        public async Task GetProductById_WhenUnknownExceptionOccursInService_ThenReturnErrorResponse()
        {
            // Arrange
            _productsRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .Throws<Exception>();

            // Act
            var result = await sut.GetAsync(1);

            // Assert
            result.Should().BeAssignableTo<Response<ProductResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Error);
            result.ErrorMessage.Should().Be(ErrorMessages.ErrorWhileRetrievingEntity);
            result.Data.Should().BeNull();

            _mapperMock.Verify(mapper => mapper.Map<ProductResponseModel>(It.IsAny<Product>()), Times.Never);
            _productsRepositoryMock.Verify(s => s.GetByIdAsync(It.IsAny<int>()), Times.Once);

            VerifyNoOtherCallsOnMocks();
        }

        [Fact]
        public async Task GetAll_WhenProductsTableIsNotEmpty_ThenReturnProductListResponse()
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
            var result = await sut.GetAllAsync();

            // Assert
            result.Should().BeAssignableTo<Response<ProductsListResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Ok);
            result.ErrorMessage.Should().BeNull();
            result.Data.Should().Be(productListResponse);

            _mapperMock.Verify(mapper => mapper.Map<ProductsListResponseModel>
                (It.IsAny<IEnumerable<Product>>()), Times.Once);
            _productsRepositoryMock.Verify(s => s.GetAllAsync(), Times.Once);

            VerifyNoOtherCallsOnMocks();
        }

        [Fact]
        public async Task GetAll_WhenProductsTableIsEmpty_ThenReturnEmptyListInResponse()
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
            var result = await sut.GetAllAsync();

            // Assert
            result.Should().BeAssignableTo<Response<ProductsListResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Ok);
            result.ErrorMessage.Should().BeNull();
            result.Data.Should().Be(productListResponse);
            result.Data.Products.Should().BeEmpty();

            _mapperMock.Verify(mapper => mapper.Map<ProductsListResponseModel>
                (It.IsAny<IEnumerable<Product>>()), Times.Once);
            _productsRepositoryMock.Verify(s => s.GetAllAsync(), Times.Once);

            VerifyNoOtherCallsOnMocks();
        }

        private void VerifyNoOtherCallsOnMocks()
        {
            _mapperMock.VerifyNoOtherCalls();
            _productsRepositoryMock.VerifyNoOtherCalls();
        }

        private void SetupGetByIdAsyncResult(Product product)
        {
            _productsRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(product);
        }

        private void SetupMapperForGetByIdAction(ProductResponseModel productResponse)
        {
            _mapperMock.Setup(mapper => mapper.Map<ProductResponseModel>(It.IsAny<Product>()))
                .Returns(productResponse);
        }

        private void SetFixtureRecursionDepth(int depth)
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior(depth));
        }
    }
}
