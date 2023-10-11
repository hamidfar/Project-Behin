namespace PD.Domain.AggregatesModel.ProductAggregate
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(Guid productId);
        Task AddProductAsync(Product product);
    }
}
