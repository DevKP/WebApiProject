using Microsoft.EntityFrameworkCore;
using WebApiProject.Domain.Entities;
using WebApiProject.Infrastructure.Configurations;
using WebApiProject.Infrastructure.Data;

namespace WebApiProject.Infrastructure.Db
{
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            Database?.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());

            modelBuilder.Entity<Category>().HasData(TablesSeed.Categories);
            modelBuilder.Entity<Product>().HasData(TablesSeed.Products);
        }
    }
}
