using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Domain.Payments
{
    public enum PaymentAttemptStatus
    {
        Pending = 1,
        Successful = 2,
        Failed = 3
    }
}
