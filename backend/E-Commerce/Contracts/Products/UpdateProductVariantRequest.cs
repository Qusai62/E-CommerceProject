namespace E_Commerce.Contracts.Products
{
    public class UpdateProductVariantRequest
    {
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}
