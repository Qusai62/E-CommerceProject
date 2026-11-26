using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.UpdatePaymentStatus
{
    public enum UpdatePaymentStatusResult
    {
        Success,
        PaymentNotFound,
        InvalidPaymentMethod,
        AlreadyPaid,
        ConcurrencyConflict
    }
}
