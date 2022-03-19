using System.Threading;
using System.Threading.Tasks;
using WebApiProject.Web.Models.Responses;

namespace WebApiProject.Web.Services
{
    public interface IProductsService
    {
        Task<Response<ProductResponseModel>> GetAsync(int id);
        Task<Response<ProductsListResponseModel>> GetAllAsync();
    }
}