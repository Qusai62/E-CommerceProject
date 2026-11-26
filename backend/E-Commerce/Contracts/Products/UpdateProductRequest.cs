namespace E_Commerce.Contracts.Products
{
    public class UpdateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }
    }
}
