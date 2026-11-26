using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.StoreOwnerViewStock
{
    public sealed record StoreOwnerStockSnapshot(
     long ProductVariantId,
     long ProductId,
     string ProductName,
     string Sku,
     bool ProductIsActive,
     bool VariantIsActive,
     int OnHandQuantity,
     int ReservedQuantity,
     int SecuredCODQuantity
 );
}
