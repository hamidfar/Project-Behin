using MediatR;
using PD.Domain.AggregatesModel.ProductAggregate;
using PD.Infrastructure.Data;

namespace PD.API.Application.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Product>
    {
        private readonly ProductDbContext _context;

        public CreateProductCommandHandler(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Title= request.Title,
                Price = request.Price,
                Categories= request.Categories,
                Description = request.Description
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }
    }

}
