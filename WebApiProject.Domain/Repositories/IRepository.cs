using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiProject.Domain.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(int id);
    }
}
