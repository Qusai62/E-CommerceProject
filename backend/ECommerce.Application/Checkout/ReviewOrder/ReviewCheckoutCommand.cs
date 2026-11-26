using ECommerce.Domain.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Checkout.ReviewOrder
{
    public sealed record ReviewCheckoutCommand(
    long SavedAddressId,
    PaymentMethod PaymentMethod
);

}
