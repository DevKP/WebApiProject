using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Domain.Repositories;
using WebApiProject.Infrastructure.Db;

namespace WebApiProject.Infrastructure.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly DatabaseContext _dbContext;

        public CategoriesRepository(DatabaseContext dbContext)
        {
            Guard.Against.Null(dbContext, nameof(dbContext));

            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int categoryId)
        {
            return await _dbContext.Categories.FindAsync(categoryId);
        }
    }
}
