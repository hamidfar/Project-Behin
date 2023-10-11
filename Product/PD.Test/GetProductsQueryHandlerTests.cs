using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using PD.Domain.AggregatesModel.ProductAggregate;
using PD.Infrastructure.Data;
using PD.API.Application.Commands;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using PD.API.Application.Queries;

namespace PD.Test
{
    public class GetProductsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnListOfProducts()
        {
            var products = new List<Product>
                {
                    new Product { Id = Guid.NewGuid(), Name = "Product 1",Categories="Category 1",Title="Product 1",Description="" },
                    new Product { Id = Guid.NewGuid(), Name = "Product 2" ,Categories="Category 1",Title="Product 2",Description=""},
                    new Product { Id = Guid.NewGuid(), Name = "Product 3",Categories="Category 1",Title="Product 3",Description="" },
                }.AsQueryable();

            var dbContext = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                .Options;

            using (var context = new ProductDbContext(dbContext))
            {
                context.Products.AddRange(products);
                context.SaveChanges();
            }

            using (var context = new ProductDbContext(dbContext))
            {
                var query = new GetProductsQuery();
                var handler = new GetProductsQueryHandler(context);

                var result = await handler.Handle(query, CancellationToken.None);

                Assert.Equal(3, result.Count);
            }
        }
    }

}
