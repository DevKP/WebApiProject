using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiProject.Domain.Entities;
using WebApiProject.Domain.Repositories;
using WebApiProject.Infrastructure.Db;

namespace WebApiProject.Infrastructure.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly DatabaseContext _dbContext;

        public ProductsRepository(DatabaseContext dbContext) => _dbContext = dbContext;

        public async Task<List<Product>> GetAllAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<Product> GetByIdAsync(int productId)
        {
            return await _dbContext.Products.FindAsync(productId);
        }

        public string GetTheMostFrequentCategoryName()
        {
            return _dbContext.Products
                .GroupBy(p => p.Category.Name, 
                    (name, products) =>
                    new
                    {
                        Count = products.Count(),
                        Name = name
                    })
                .OrderByDescending(p => p.Count)
                .First().Name;
        }
    }
}
