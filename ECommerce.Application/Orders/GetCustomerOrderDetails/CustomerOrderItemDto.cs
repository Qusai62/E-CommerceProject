using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Orders.GetCustomerOrderDetails
{
    public sealed record CustomerOrderItemDto(
     long OrderItemId,
     long ProductVariantId,
     string ProductName,
     string Sku,
     string VariantAttributes,
     int Quantity,
     decimal UnitPrice
 )
    {
        public decimal Subtotal => Quantity * UnitPrice;
    }
}
