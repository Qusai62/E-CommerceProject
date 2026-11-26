namespace E_Commerce.Contracts.Products
{
    public class UpdateProductImageRequest
    {
        public string? AltText { get; set; }

        public int SortOrder { get; set; }

        public bool IsPrimary { get; set; }
    }
}
