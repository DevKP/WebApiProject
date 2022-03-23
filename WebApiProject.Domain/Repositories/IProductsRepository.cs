using System.Threading.Tasks;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Domain.Repositories
{
    public interface IProductsRepository : IRepository<Product>
    {
        Task<string> GetTheMostFrequentCategoryNameAsync();
    }
}
