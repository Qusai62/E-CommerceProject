using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.SupplierInvoices.RecordSupplierPayment
{
    public sealed class SupplierPaymentConcurrencyException : Exception
    {
        public SupplierPaymentConcurrencyException(Exception innerException)
            : base("The supplier invoice was modified concurrently.",
                   innerException)
        {
        }
    }

    public sealed class DuplicateSupplierPaymentSubmissionException : Exception
    {
        public DuplicateSupplierPaymentSubmissionException(Exception innerException)
            : base("The payment submission already exists.",
                   innerException)
        {
        }
    }
}
