using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Response
{
    public class ProductActivationResponse
    {
        public long ProductId { get; init; }

        public bool IsActive { get; init; }

        public DateTime? UpdatedAtUtc { get; init; }
    }
}
