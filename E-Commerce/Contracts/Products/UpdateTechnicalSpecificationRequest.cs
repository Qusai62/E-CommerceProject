namespace E_Commerce.Contracts.Products
{
    public class UpdateTechnicalSpecificationRequest
    {
        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public int SortOrder { get; set; }
    }
}
