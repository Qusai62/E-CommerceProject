using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.TechnicalSpecification.AddTechnicalSpecification
{
    public class AddTechnicalSpecificationResponse
    {
        public long TechnicalSpecificationId { get; set; }

        public long ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public DateTime CreatedAtUtc { get; set; }
    }
}
