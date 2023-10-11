using Microsoft.EntityFrameworkCore;
using Moq;
using PD.Domain.AggregatesModel.ProductAggregate;
using PD.Infrastructure.Data;
using PD.Infrastructure.Repositories;

namespace PD.Test
{


    public class ProductRepositoryTests
    {
        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            var products = new List<Product>
        {
            new Product { Id = Guid.NewGuid(), Name = "Product 1",Categories="Category 1",Title="Product 1",Description="" },
            new Product { Id = Guid.NewGuid(), Name = "Product 2" ,Categories="Category 1",Title="Product 2",Description=""},
            new Product { Id = Guid.NewGuid(), Name = "Product 3",Categories="Category 1",Title="Product 3",Description="" },
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

            var options = new DbContextOptionsBuilder<ProductDbContext>()
              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
              .Options;

            using (var context = new ProductDbContext(options))
            {
                context.Products.AddRange(products);
                context.SaveChanges();
            }

            var dbContext = new ProductDbContext(options);

            var repository = new ProductRepository(dbContext);

            var result = await repository.GetAllProductsAsync();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldReturnProductWithMatchingId()
        {
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Product 1", Categories = "Category 1", Title = "Product 1", Description = "" };

            var options = new DbContextOptionsBuilder<ProductDbContext>()
              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
              .Options;

            using (var context = new ProductDbContext(options))
            {
                context.Products.Add(product);
                context.SaveChanges();
            }

            var dbContext = new ProductDbContext(options);

            var repository = new ProductRepository(dbContext);

            var result = await repository.GetProductByIdAsync(productId);

            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
        }

        [Fact]
        public async Task AddProductAsync_ShouldAddProductToDatabase()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "Product 1", Categories = "Category 1", Title = "Product 1", Description = "" };


            var options = new DbContextOptionsBuilder<ProductDbContext>()
              .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
              .Options;


            var dbContext = new ProductDbContext(options);

            var repository = new ProductRepository(dbContext);

            await repository.AddProductAsync(product);

            var addedProduct = await dbContext.Products.FirstOrDefaultAsync();
            Assert.NotNull(addedProduct);
            Assert.Equal(product.Id, addedProduct.Id);
        }
    }

}