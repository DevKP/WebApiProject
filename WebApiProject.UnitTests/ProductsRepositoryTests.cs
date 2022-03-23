using System;
using System.Linq;
using System.Threading.Tasks;
using EntityFrameworkCore.Testing.NSubstitute;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Db;
using WebApiProject.Infrastructure.Repositories;
using WebApiProject.UnitTests.TableData;
using Xunit;

namespace WebApiProject.UnitTests
{
    public sealed class ProductsRepositoryTests : DatabaseContextTests
    {
        private readonly ProductsRepository _sut;

        public ProductsRepositoryTests()
        {
            _sut = new ProductsRepository(_databaseContext);

            ClearDatabase();
            SeedDatabase();
        }

        [Fact]
        public void Ctor_WhenDbContextIsNull_ThenThrowArgumentNullException()
        {
            Action action = () => new ProductsRepository(null);
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("dbContext");
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductExistsInDb_ThenReturnThatProduct()
        {
            // Arrange
            const int productId = 3;
            var expectedResult = TableTestData.Products.FirstOrDefault(p => p.Id == productId);

            // Act
            var result = await _sut.GetByIdAsync(productId);

            // Assert
            result.Should().BeEquivalentTo(expectedResult,
                config => config.Excluding(p => p.Category));
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductDoNotExistsInDb_ThenReturnNull()
        {
            // Arrange
            const int productId = 69;

            // Act
            var result = await _sut.GetByIdAsync(productId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_WhenProductsExistsInDb_ThenReturnAllProducts()
        {
            // Arrange

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(TableTestData.Products, 
                config => config.Excluding(p => p.Category));
        }

        [Fact]
        public async Task GetTheMostFrequentCategoryName_ShouldReturnTrueCategoryName()
        {
            // Arrange

            // Act
            var result = await _sut.GetTheMostFrequentCategoryNameAsync();

            // Assert
            result.Should().Be(TableTestData.MostFrequentCategory);
        }

       
    }
}