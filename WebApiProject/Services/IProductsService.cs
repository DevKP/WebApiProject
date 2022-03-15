using System.Threading.Tasks;
using WebApiProject.Web.Models.Responses;

namespace WebApiProject.Web.Services
{
    public interface IProductsService
    {
        Task<ProductResponseModel> GetAsync(int id);
        Task<ProductsListResponseModel> GetAllAsync();
    }
}