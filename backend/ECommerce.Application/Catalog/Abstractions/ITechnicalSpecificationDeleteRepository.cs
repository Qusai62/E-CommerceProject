using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Abstractions
{
    public interface ITechnicalSpecificationDeleteRepository
    {
        Task<bool> DeleteAsync(
        long productId,
        long specificationId,
        CancellationToken cancellationToken = default);
    }
}
