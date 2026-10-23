using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Product.CreateProduct
{
    public class CreateProductResponse
    {
        public long ProductId { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; }=null!;
        public bool IsActive { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
