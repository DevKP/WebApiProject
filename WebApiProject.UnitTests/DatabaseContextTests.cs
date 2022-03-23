using System;
using EntityFrameworkCore.Testing.NSubstitute;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Db;
using WebApiProject.Infrastructure.Repositories;
using WebApiProject.UnitTests.TableData;

namespace WebApiProject.UnitTests
{
    public abstract class DatabaseContextTests
    { 
        protected readonly DatabaseContext _databaseContext;

        protected DatabaseContextTests()
        {
            var dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _databaseContext = Create.MockedDbContextFor<DatabaseContext>(dbContextOptions);
        }

        protected virtual void SeedDatabase()
        {
            _databaseContext.Categories.AddRange(TableTestData.Categories);
            _databaseContext.Products.AddRange(TableTestData.Products);
            _databaseContext.SaveChanges();
        }

        protected virtual void ClearDatabase()
        {
            ClearTable(_databaseContext.Products);
            ClearTable(_databaseContext.Categories);
        }

        protected void ClearTable<T>(DbSet<T> dbSet) where T : class, IEntity
        {
            dbSet.RemoveRange(dbSet);
            _databaseContext.SaveChanges();
        }
    }
}