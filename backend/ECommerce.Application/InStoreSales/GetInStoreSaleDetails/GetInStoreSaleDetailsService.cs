using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.InStoreSales.GetInStoreSaleDetails
{
    public sealed class GetInStoreSaleDetailsService
    {
        private readonly IGetInStoreSaleDetailsRepository _repository;

        public GetInStoreSaleDetailsService(
            IGetInStoreSaleDetailsRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetInStoreSaleDetailsResponse?> ExecuteAsync(
            long inStoreSaleId,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate sale identifier
            if (inStoreSaleId <= 0)
            {
                throw new ArgumentException(
                    "InStoreSaleId must be greater than zero.");
            }

            // 2. Retrieve sale details
            var sale = await _repository.GetByIdAsync(
                inStoreSaleId,
                cancellationToken);

            // 3. Return sale or null
            return sale;
        }
    }
}
