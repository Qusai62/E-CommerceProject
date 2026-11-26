using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.SelectPaymentMethod
{
    public class PaymentMethodValidator:IPaymentMethodValidator
    {
        public bool IsSupported(PaymentMethod paymentMethod)
        {
            return paymentMethod is
                PaymentMethod.OnlinePayment or
                PaymentMethod.CashOnDelivery;
        }
    }
}
