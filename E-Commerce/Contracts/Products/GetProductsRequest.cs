namespace E_Commerce.Contracts.Products
{
    public class GetProductsRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string? Search { get; set; }

        public long? BrandId { get; set; }
        public long? CategoryId { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public string SortBy { get; set; } = "newest";
        public string SortDirection { get; set; } = "desc";
    }
}
