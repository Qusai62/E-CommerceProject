namespace E_Commerce.Contracts.Brands
{
    public sealed class GetBrandsRequest
    {
        public int PageNumber { get; init; } = 1;

        public int PageSize { get; init; } = 20;

        public string? Search { get; init; }

        public bool? IsActive { get; init; }
    }
}
