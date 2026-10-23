using ECommerce.Application.Catalog.Product.GetProductById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Abstractions
{
    public interface IProductReadRepository
    {
        Task<ProductResponse?> GetByIdAsync(long productId,CancellationToken cancellationToken = default);
    }
}
