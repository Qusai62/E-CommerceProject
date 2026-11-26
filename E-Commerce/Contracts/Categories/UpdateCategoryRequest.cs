namespace E_Commerce.Contracts.Categories
{
    public sealed record UpdateCategoryRequest(
            string Name,
            string? Description,
            int SortOrder);
}
