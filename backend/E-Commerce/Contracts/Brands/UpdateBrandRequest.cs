using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Contracts.Brands
{
    public sealed class UpdateBrandRequest
    {
        [Required]
        [StringLength(150, MinimumLength = 1)]
        public string Name { get; init; } = null!;

        [StringLength(1000)]
        public string? Description { get; init; }

        [StringLength(500)]
        public string? LogoStorageKey { get; init; }
    }
}
