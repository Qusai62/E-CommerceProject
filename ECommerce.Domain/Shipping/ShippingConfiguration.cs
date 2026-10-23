using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Shipping
{
    public class ShippingConfiguration
    {
        public long ShippingConfigurationId { get; set; }

        public ShippingMode ShippingMode { get; set; }

        public decimal ShippingCharge { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
