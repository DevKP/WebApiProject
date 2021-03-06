using WebApiProject.Domain.Entities;

namespace WebApiProject.UnitTests.TableData
{
    static class TableTestData
    {
        public const string MostFrequentCategory = "Cosmetics";

        public static Category[] Categories =>
            new[]
            {
                new Category { Id = 1, Name = "Food"},
                new Category { Id = 2, Name = "Electronics"},
                new Category { Id = 3, Name = "Cosmetics"}
            };


        public static Product[] Products =>
            new[]
            {
                new Product { Id = 1, Name = "Tea", IsAvailable = true, Price = 19.99M, CategoryId = 1 },
                new Product { Id = 2, Name = "Milk", IsAvailable = true, Price = 30.00M, CategoryId = 1 },
                new Product { Id = 3, Name = "Bread", IsAvailable = false, Price = 18.20M, CategoryId = 1 },

                new Product { Id = 4, Name = "Sony Xperia 1", IsAvailable = false, Price = 16420.75M, CategoryId = 2 },
                new Product { Id = 5, Name = "Xiaomi Redmi 9", IsAvailable = true, Price = 6969.42M, CategoryId = 2 },
                new Product { Id = 6, Name = "Meizu m8 Note", IsAvailable = false, Price = 5999.99M, CategoryId = 2 },

                new Product { Id = 7, Name = "Cream", IsAvailable = true, Price = 100.01M, CategoryId = 3 },
                new Product { Id = 8, Name = "Shampoo", IsAvailable = true, Price = 59.80M, CategoryId = 3 },
                new Product { Id = 9, Name = "Tonic", IsAvailable = false, Price = 80.14M, CategoryId = 3 },
                new Product { Id = 10, Name = "Eyeshadow", IsAvailable = true, Price = 401.25M, CategoryId = 3 }
            };
    }
}
