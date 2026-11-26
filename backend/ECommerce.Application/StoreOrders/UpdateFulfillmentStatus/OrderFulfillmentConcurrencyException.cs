using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.UpdateFulfillmentStatus
{
    public sealed class OrderFulfillmentConcurrencyException
     : Exception
    {
        public OrderFulfillmentConcurrencyException(
            string message,
            Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
