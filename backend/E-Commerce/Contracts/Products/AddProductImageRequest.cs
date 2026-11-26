namespace E_Commerce.Contracts.Products
{
    public class AddProductImageRequest
    {
        public IFormFile File { get; set; } = null!;

        public string? AltText { get; set; }

        public int SortOrder { get; set; }
    }
}
