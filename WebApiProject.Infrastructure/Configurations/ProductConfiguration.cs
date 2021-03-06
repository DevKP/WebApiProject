using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Infrastructure.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(product => product.Id);
            builder.Property(product => product.Name).IsRequired().HasMaxLength(150);
            builder.Property(product => product.Price).IsRequired().HasPrecision(18, 2);
            builder.Property(product => product.IsAvailable).IsRequired();

            builder.HasOne(product => product.Category)
                   .WithMany(category => category.Products)
                   .HasForeignKey(product => product.CategoryId);
        }
    }
}
