﻿using System;
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

        public IEnumerable<Product> GetAll()
        {
            return _dbContext.Products.ToList();
        }

        public Product GetById(int productId)
        {
            return _dbContext.Products.FirstOrDefault(p => p.Id == productId);
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
                        .First().Name;
        }
    }
}
