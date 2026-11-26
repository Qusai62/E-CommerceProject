using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.StoreOwnerViewStock
{
    public sealed record StoreOwnerStockResponse(
    long ProductVariantId,
    long ProductId,
    string ProductName,
    string Sku,
    bool ProductIsActive,
    bool VariantIsActive,
    int OnHandQuantity,
    int ReservedQuantity,
    int SecuredCODQuantity,
    int AvailableQuantity,
    bool IsInStock
);
}
