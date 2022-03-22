using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using EntityFrameworkCore.Testing.NSubstitute;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Db;
using WebApiProject.Infrastructure.Repositories;
using WebApiProject.UnitTests.Extensions;
using WebApiProject.UnitTests.TableData;
using Xunit;

namespace WebApiProject.UnitTests
{
    public class ProductsRepositoryTests
    {
        private readonly ProductsRepository _sut;
        private readonly DatabaseContext _databaseContext;
        private readonly Fixture _fixture = new();

        public ProductsRepositoryTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = Create.MockedDbContextFor<DatabaseContext>(dbContextOptions);
            _sut = new ProductsRepository(_databaseContext);
            _fixture.SetFixtureRecursionDepth(1);

            ClearDatabase();
            SeedDatabase();
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

        private void SeedDatabase()
        {
            _databaseContext.Categories.AddRange(TableTestData.Categories);
            _databaseContext.Products.AddRange(TableTestData.Products);
            _databaseContext.SaveChanges();
        }

        private void ClearDatabase()
        {
            ClearTable(_databaseContext.Products);
            ClearTable(_databaseContext.Categories);
        }

        private void ClearTable<T>(DbSet<T> dbSet) where T: class
        {
            dbSet.RemoveRange(dbSet);
            _databaseContext.SaveChanges();
        }
    }
}