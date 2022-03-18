using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiProject.Domain.Repositories
{
    public interface IRepository<T> where T : class, new()
    {
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<T> GetByIdAsync(int id);
    }
}
