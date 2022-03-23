using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using WebApiProject.Domain.Entities;
using WebApiProject.Domain.Repositories;
using WebApiProject.UnitTests.Extensions;
using WebApiProject.Web.Data;
using WebApiProject.Web.Models.Responses;
using WebApiProject.Web.Services;
using Xunit;

namespace WebApiProject.UnitTests
{
    public class ProductsServiceTests
    {
        private readonly ProductsService _sut;
        private readonly IProductsRepository _productsRepository = Substitute.For<IProductsRepository>();
        private readonly ILogger<IProductsService> _logger = Substitute.For<ILogger<IProductsService>>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly Fixture _fixture;

        public ProductsServiceTests()
        {
            _sut = new ProductsService(_productsRepository, _mapper, _logger);
            _fixture = new Fixture();
            
            _fixture.SetFixtureRecursionDepth(1);
        }

        [Fact]
        public void Ctor_WhenProductsRepositoryIsNull_ThenThrowArgumentNullException()
        {
            Action action = () => new ProductsService(null, _mapper, _logger);
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("productsRepository");
        }

        [Fact]
        public void Ctor_WhenMapperIsNull_ThenThrowArgumentNullException()
        {
            Action action = () => new ProductsService(_productsRepository, null, _logger);
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("mapper");
        }

        [Fact]
        public void Ctor_WhenLoggerIsNull_ThenThrowArgumentNullException()
        {
            Action action = () => new ProductsService(_productsRepository, _mapper, null);
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("logger");
        }

        [Fact]
        public async Task GetProductById_WhenProductExists_ThenReturnOkProductResponse()
        {
            // Arrange
            const int productId = 1;
            var product = _fixture.Create<Product>();

            var productResponse = new ProductResponseModel()
            {
                Id = product.Id,
                Name = product.Name,
                IsAvailable = product.IsAvailable,
                Price = product.Price,
                CategoryName = "Test category"
            };

            _productsRepository.GetByIdAsync(productId).Returns(product);
            _mapper.Map<ProductResponseModel>(Arg.Any<Product>()).Returns(productResponse);

            // Act
            var result = await _sut.GetAsync(productId);

            // Assert
            result.Should().BeAssignableTo<Response<ProductResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Ok);
            result.ErrorMessage.Should().BeNull();
            result.Data.Should().BeEquivalentTo(productResponse);

            await _productsRepository.Received(1).GetByIdAsync(productId);
            _mapper.Received(1).Map<ProductResponseModel>(Arg.Any<Product>());
        }

        [Fact]
        public async Task GetProductById_WhenRepositoryReturnsNull_ThenReturnNotFoundResponse()
        {
            // Arrange
            const int productId = 1;

            _productsRepository.GetByIdAsync(productId).ReturnsNull();
            _mapper.Map<ProductResponseModel>(Arg.Any<Product>()).ReturnsNull();

            // Act
            var result = await _sut.GetAsync(productId);

            // Assert
            result.Should().BeAssignableTo<Response<ProductResponseModel>>();
            result.Status.Should().Be(ResponseStatus.NotFound);
            result.ErrorMessage.Should().Be(ErrorMessages.NotFoundInDatabase);
            result.Data.Should().BeNull();

            await _productsRepository.Received(1).GetByIdAsync(productId);
            _mapper.Received(0).Map<ProductResponseModel>(Arg.Any<Product>());
        }

        [Fact]
        public async Task GetProductById_WhenUnknownExceptionOccursInService_ThenReturnErrorResponse()
        {
            // Arrange
            const int productId = 1;

            _productsRepository.GetByIdAsync(productId).Throws<Exception>();
            _mapper.Map<ProductResponseModel>(Arg.Any<Product>()).ReturnsNull();

            // Act
            var result = await _sut.GetAsync(productId);

            // Assert
            result.Should().BeAssignableTo<Response<ProductResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Error);
            result.ErrorMessage.Should().Be(ErrorMessages.ErrorWhileRetrievingEntity);
            result.Data.Should().BeNull();

            await _productsRepository.Received(1).GetByIdAsync(productId);
            _mapper.Received(0).Map<ProductResponseModel>(Arg.Any<Product>());
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

            _productsRepository.GetAllAsync().Returns(products);
            _mapper.Map<ProductsListResponseModel>(Arg.Any<IEnumerable<Product>>()).Returns(productListResponse);

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().BeAssignableTo<Response<ProductsListResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Ok);
            result.ErrorMessage.Should().BeNull();
            result.Data.Should().BeEquivalentTo(productListResponse);

            await _productsRepository.Received(1).GetAllAsync();
            _mapper.Received(1).Map<ProductsListResponseModel>(Arg.Any<IEnumerable<Product>>());
        }

        [Fact]
        public async Task GetAll_WhenProductsTableIsEmpty_ThenReturnEmptyListInResponse()
        {
            // Arrange
            var productListResponse = new ProductsListResponseModel()
            {
                Products = new List<ProductResponseModel>()
            };

            _productsRepository.GetAllAsync().Returns(new List<Product>());
            _mapper.Map<ProductsListResponseModel>(Arg.Any<IEnumerable<Product>>()).Returns(productListResponse);

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().BeAssignableTo<Response<ProductsListResponseModel>>();
            result.Status.Should().Be(ResponseStatus.Ok);
            result.ErrorMessage.Should().BeNull();
            result.Data.Products.Should().BeEmpty();

            await _productsRepository.Received(1).GetAllAsync();
            _mapper.Received(1).Map<ProductsListResponseModel>(Arg.Any<IEnumerable<Product>>());
        }
    }
}
