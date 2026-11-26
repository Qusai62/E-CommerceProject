using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Product.GetProductById
{
    public class ProductVariantResponse
    {
        public long ProductVariantId {  get; set; }
        public decimal Price { get; set; }
       
        public IReadOnlyList<VariantAttributeResponse> VariantAttributeResponses { get; set; }
    }
}
