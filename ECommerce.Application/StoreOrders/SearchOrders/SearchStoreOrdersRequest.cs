using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.StoreOrders.SearchOrders
{
    public sealed class SearchStoreOrdersRequest
    {
        public string OrderNumber { get; init; } = string.Empty;
    }
}
