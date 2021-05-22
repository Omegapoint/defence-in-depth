namespace Defence.In.Depth.Domain.Model
{
    public class Product
    {
        public Product(ProductId id, ProductName name)
        {
            Id = id;
            Name = name;
        }

        public ProductId Id { get; }

        public ProductName Name { get; }
    }
}