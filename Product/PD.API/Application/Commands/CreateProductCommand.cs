using MediatR;
using PD.Domain.AggregatesModel.ProductAggregate;

namespace PD.API.Application.Commands
{
    public class CreateProductCommand : IRequest<Product>
    {
        public string Name { get; set; }
        public string Categories { get; set; }
        public decimal Price { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
