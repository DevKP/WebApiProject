using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiProject.Domain.Entities;

namespace WebApiProject.Infrastructure.Configurations
{
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(category => category.Id);
            builder.Property(category => category.Name).IsRequired().HasMaxLength(150);

            builder.HasMany(category => category.Products)
                   .WithOne(product => product.Category);
        }
    }
}
