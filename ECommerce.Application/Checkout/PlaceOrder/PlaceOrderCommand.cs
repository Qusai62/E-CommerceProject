using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.PlaceOrder
{
    public sealed record PlaceOrderCommand(
     Guid CheckoutSubmissionId,
     long SavedAddressId,
     PaymentMethod PaymentMethod
 );
}
