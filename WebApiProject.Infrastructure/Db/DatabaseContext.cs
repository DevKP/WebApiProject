using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Infrastructure.Db
{
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<Product> Products { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(GetProductsSeed());
        }

        private static Product[] GetProductsSeed()
        {
            return new[]
            {
                new Product { Id = 1, Name = "Tea", IsAvailable = true, Price = 1.99M },
                new Product { Id = 2, Name = "Milk", IsAvailable = true, Price = 0.99M },
                new Product { Id = 3, Name = "Carpet", IsAvailable = false, Price = 0.50M },
                new Product { Id = 4, Name = "Bread", IsAvailable = false, Price = 0.75M },
                new Product { Id = 5, Name = "Shoes", IsAvailable = true, Price = 5.99M }
            };
        }
    }
}
