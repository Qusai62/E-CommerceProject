namespace ECommerce.Application.Catalog.Abstractions;

public sealed class ProductSlugConflictException : Exception
{
    public ProductSlugConflictException(
        string slug,
        Exception innerException)
        : base(
            $"The product slug '{slug}' is already in use.",
            innerException)
    {
    }
}