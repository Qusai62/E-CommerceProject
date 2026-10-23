using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Product.GetProductById
{
    public class TechnicalSpecificationResponse
    {
        public long TechnicalSpecificationId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public int SortOrder { get; set; }
    }
}
