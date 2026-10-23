namespace E_Commerce.Contracts.Products
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }
    }
}
