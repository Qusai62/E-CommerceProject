using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Product.GetProductById
{
    public class ProductImageResponse
    {
        public long ProductImageId { get; init; }

        public required string ImageUrl { get; init; }

        public string? AltText { get; init; }

        public int SortOrder { get; init; }

        public bool IsPrimary { get; init; }

    }
}
