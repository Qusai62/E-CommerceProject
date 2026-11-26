using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.ViewOrderDetails
{
    public sealed record StoreOrderItemResponse(
     long OrderItemId,
     long ProductVariantId,
     string ProductName,
     string Sku,
     string VariantAttributes,
     int Quantity,
     decimal UnitPrice
 );
}
