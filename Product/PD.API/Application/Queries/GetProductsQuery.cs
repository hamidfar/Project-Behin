using MediatR;
using PD.Domain.AggregatesModel.ProductAggregate;

namespace PD.API.Application.Queries
{
    public class GetProductsQuery : IRequest<List<Product>>
    {
    }
}
