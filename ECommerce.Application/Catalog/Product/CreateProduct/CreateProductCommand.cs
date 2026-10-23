using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Product.CreateProduct
{
    public class CreateProductCommand
    {
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public long? BrandId { get; set; }

        public long? CategoryId { get; set; }
    }
}
