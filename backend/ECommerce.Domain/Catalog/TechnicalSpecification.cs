using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Catalog
{
    public class TechnicalSpecification
    {
        public long TechnicalSpecificationId { get; set; }

        public long ProductId { get; set; }

        public string Name { get; set; } = null!;

        public string Value { get; set; } = null!;

        public int SortOrder { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }

        public Product Product { get; set; } = null!;
    }
}
