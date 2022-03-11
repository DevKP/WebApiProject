using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiProject.Domain.Repositories
{
    public interface IRepository<T> where T : class, new()
    {
        IEnumerable<T> GetAll();

        T GetById(int id);
    }
}
