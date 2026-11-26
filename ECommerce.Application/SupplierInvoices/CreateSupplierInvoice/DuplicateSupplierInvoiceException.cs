using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.CreateSupplierInvoice
{
    public sealed class DuplicateSupplierInvoiceException
      : Exception
    {
        public DuplicateSupplierInvoiceException()
            : base("This supplier invoice number already exists.")
        {
        }

        public DuplicateSupplierInvoiceException(
            Exception innerException)
            : base(
                "This supplier invoice number already exists.",
                innerException)
        {
        }
    }
}
