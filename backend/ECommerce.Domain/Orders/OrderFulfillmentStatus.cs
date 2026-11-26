using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Orders
{
   
        public enum OrderFulfillmentStatus
        {
            Pending = 1,
            Processing = 2,
            Fulfilled = 3,
            Cancelled = 4
        }
}
