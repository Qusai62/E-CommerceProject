using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.TechnicalSpecification.UpdateTechnicalSpecification
{
    public class UpdateTechnicalSpecificationCommand
    {
        public long ProductId { get; set; }

        public long SpecificationId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public int SortOrder { get; set; }
    }
}
