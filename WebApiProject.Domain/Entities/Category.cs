using System.Collections.Generic;

namespace WebApiProject.Domain.Entities
{
    public class Category : IEntity
    {
        public int Id { get; init; }
        public string Name { get; init; }

        public virtual ICollection<Product> Products { get; init; }
    }
}
