using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Product.GetProductById
{
    public class VariantAttributeResponse
    {
        public string Name {  get; set; }
        public string Value { get; set; }
        public int SortOrder { get; set; }
    }
}
