using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.GetSupplierById
{
    public sealed class GetSupplierByIdService
    {
        private readonly IGetSupplierByIdRepository _repository;

        public GetSupplierByIdService(
            IGetSupplierByIdRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<SupplierResponse>> ExecuteAsync(
            long supplierId,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate the supplier ID
            if (supplierId <= 0)
            {
                return Result<SupplierResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Supplier.InvalidId",
                        "Supplier ID must be greater than zero.")
                    });
            }

            // Step 2: Retrieve supplier from repository
            var supplier = await _repository.GetByIdAsync(
                supplierId,
                cancellationToken);

            // Step 3: Supplier does not exist
            if (supplier is null)
            {
                return Result<SupplierResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Supplier.NotFound",
                        "The requested supplier was not found.")
                    });
            }

            // Step 4: Return success
            return Result<SupplierResponse>.Success(supplier);
        }
    }
}
