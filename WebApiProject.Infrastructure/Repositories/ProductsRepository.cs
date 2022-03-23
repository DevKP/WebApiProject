using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using WebApiProject.Domain.Entities;
using WebApiProject.Domain.Repositories;
using WebApiProject.Infrastructure.Db;

namespace WebApiProject.Infrastructure.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly DatabaseContext _dbContext;

        public ProductsRepository(DatabaseContext dbContext)
        {
            Guard.Against.Null(dbContext, nameof(dbContext));

            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int productId)
        {
            return await _dbContext.Products.FindAsync(productId);
        }

        public async Task<string> GetTheMostFrequentCategoryNameAsync()
        {
            return (await _dbContext.Products
                .GroupBy(p => p.Category.Name, 
                    (name, products) =>
                    new
                    {
                        Count = products.Count(),
                        Name = name
                    })
                .OrderByDescending(p => p.Count)
                .FirstAsync()).Name;
        }
    }
}
