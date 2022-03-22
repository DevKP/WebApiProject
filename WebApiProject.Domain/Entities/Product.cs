using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiProject.Domain.Entities
{
    public class Product : IEntity
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public bool IsAvailable { get; init; }
        public decimal Price { get; init; }

        public int CategoryId { get; init; }
        public virtual Category Category { get; init; }
    }
}
