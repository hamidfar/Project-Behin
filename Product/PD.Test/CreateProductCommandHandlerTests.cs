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

namespace PD.Test
{
    public class CreateProductCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateProduct()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            var dbContext = new ProductDbContext(options);

            var createProductCommand = new CreateProductCommand
            {
                Name = "Test Product",
                Title = "Test Title",
                Price = 100,
                Categories = "Test Category",
                Description = "Test Description"
            };
            var handler = new CreateProductCommandHandler(dbContext);

            var result = await handler.Handle(createProductCommand, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(createProductCommand.Name, result.Name);
            Assert.Equal(createProductCommand.Title, result.Title);
            Assert.Equal(createProductCommand.Price, result.Price);
            Assert.Equal(createProductCommand.Categories, result.Categories);
            Assert.Equal(createProductCommand.Description, result.Description);

            Assert.Equal(1, dbContext.Products.Count()); 
        }
    }

}
