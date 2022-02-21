using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Domain.Repositories
{
    public interface IProductsRepository
    {
        IEnumerable<Product> GetAll();

        Product GetById(int productId);

        void Insert(Product product);

        void Remove(Product product);
    }
}
