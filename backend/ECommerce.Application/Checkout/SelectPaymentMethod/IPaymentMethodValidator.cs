using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.SelectPaymentMethod
{
    public interface IPaymentMethodValidator
    {
        bool IsSupported(PaymentMethod paymentMethod);
    }
}
