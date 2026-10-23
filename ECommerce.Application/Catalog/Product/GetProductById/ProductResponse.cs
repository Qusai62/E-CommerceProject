using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Product.GetProductById
{
    public class ProductResponse
    {
        public long ProductId { get; set; }

        public string ProductName { get; set; }
        public string? Description { get; set; }
        public string BrandName { get; set; }
        public IReadOnlyList<ProductImageResponse> Images { get; set; } = [];//Because a response DTO generally doesn't need to expose methods
                                                                                    //such as Add() or Remove() to its consumers
        public IReadOnlyList<ProductVariantResponse> Variants { get; set; }= [];

        public IReadOnlyList<TechnicalSpecificationResponse> TechnicalSpecifications { get; set; } = [];

    }
}
