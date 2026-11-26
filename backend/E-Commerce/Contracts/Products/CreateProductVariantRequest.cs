namespace E_Commerce.Contracts.Products
{
    public class CreateProductVariantRequest
    {
        public string Sku { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public List<VariantAttributeRequest> Attributes { get; set; } = new();
    }
}
