using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.ListSupplierInvoices
{
    public interface IListSupplierInvoicesRepository
    {
        Task<PagedSupplierInvoicesResponse> GetPagedAsync(
            ListSupplierInvoicesRequest request,
            CancellationToken cancellationToken = default);
    }
}
