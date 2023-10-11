namespace PD.Domain.AggregatesModel.ProductAggregate
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Categories { get; set; }
        public decimal Price { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
