using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Inventory.GetVariantAvailability
{
    public sealed record VariantStockSnapshot( //This class represents a read-only snapshot of inventory information. We do not intend to modify it or save it as an EF Core entity.
    
        long ProductVariantId,
    int OnHandQuantity,
    int ReservedQuantity,
    int SecuredCODQuantity
    );
}
