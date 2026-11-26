using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.ListSuppliers
{
    public interface IListSuppliersRepository
    {
        Task<PagedSuppliersResponse> GetPagedAsync(
            ListSuppliersRequest request,
            CancellationToken cancellationToken = default);
    }
}
