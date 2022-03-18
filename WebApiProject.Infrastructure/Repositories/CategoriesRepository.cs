using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Domain.Repositories;
using WebApiProject.Infrastructure.Db;

namespace WebApiProject.Infrastructure.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly DatabaseContext _dbContext;

        public CategoriesRepository(DatabaseContext dbContext) => _dbContext = dbContext;

        public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Categories.ToListAsync(cancellationToken);
        }

        public async Task<Category> GetByIdAsync(int categoryId)
        {
            return await _dbContext.Categories.FindAsync(categoryId);
        }
    }
}
