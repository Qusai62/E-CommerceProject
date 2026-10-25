using ECommerce.Application.Catalog.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.Product.GetProductById
{
    public class GetProductByIdService
    {
        private readonly IProductReadRepository _productReadRepository;

        public GetProductByIdService(
            IProductReadRepository productReadRepository)
        {
            _productReadRepository = productReadRepository;
        }

        public async Task<ProductResponse?> ExecuteAsync(
            long productId,
            CancellationToken cancellationToken = default)
        {
            return await _productReadRepository.GetByIdAsync(
                productId,
                cancellationToken);
        }
    }
}
