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
    public class CategoriesRepositoryTests
    {
        private readonly CategoriesRepository _sut;
        private readonly DatabaseContext _databaseContext;

        public CategoriesRepositoryTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = Create.MockedDbContextFor<DatabaseContext>(dbContextOptions);
            _sut = new CategoriesRepository(_databaseContext);

            ClearDatabase();
            SeedDatabase();
        }

        [Fact]
        public void Ctor_WhenDbContextIsNull_ThenThrowArgumentNullException()
        {
            Action action = () => new CategoriesRepository(null);
            action.Should().Throw<ArgumentNullException>()
                .Which.ParamName.Should().Be("dbContext");
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoriesExistsInDb_ThenReturnThatProduct()
        {
            // Arrange
            const int categoryId = 3;
            var expectedResult = TableTestData.Categories.FirstOrDefault(p => p.Id == categoryId);

            // Act
            var result = await _sut.GetByIdAsync(categoryId);

            // Assert
            result.Should().BeEquivalentTo(expectedResult,
                config => config.Excluding(c => c.Products));
        }

        [Fact]
        public async Task GetByIdAsync_WhenCategoriesDoNotExistsInDb_ThenReturnNull()
        {
            // Arrange
            const int categoryId = 69;

            // Act
            var result = await _sut.GetByIdAsync(categoryId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_WhenCategoriesExistsInDb_ThenReturnAllProducts()
        {
            // Arrange

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(TableTestData.Categories, 
                config => config.Excluding(c => c.Products));
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

        private void ClearTable<T>(DbSet<T> dbSet) where T: class, IEntity
        {
            dbSet.RemoveRange(dbSet);
            _databaseContext.SaveChanges();
        }
    }
}