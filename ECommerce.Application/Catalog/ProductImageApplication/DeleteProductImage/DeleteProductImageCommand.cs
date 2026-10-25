using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.ProductImageApplication.DeleteProductImage
{
    public class DeleteProductImageCommand
    {
        public long ProductId { get; set; }

        public long ProductImageId { get; set; }
    }
}
