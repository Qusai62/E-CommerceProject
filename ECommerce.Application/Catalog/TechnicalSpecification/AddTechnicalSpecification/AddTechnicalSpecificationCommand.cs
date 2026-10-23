using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.TechnicalSpecification.AddTechnicalSpecification
{
    public class AddTechnicalSpecificationCommand
    {
        public long ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public int SortOrder { get; set; }
    }
}
