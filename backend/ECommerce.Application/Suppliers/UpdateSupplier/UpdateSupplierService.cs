using ECommerce.Application.Common.Results;
using ECommerce.Application.Suppliers.GetSupplierById;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.UpdateSupplier
{
    public sealed class UpdateSupplierService
    {
        private readonly IUpdateSupplierRepository _repository;
        private readonly TimeProvider _timeProvider;

        public UpdateSupplierService(
            IUpdateSupplierRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<Result<SupplierResponse>> ExecuteAsync(
            long supplierId,
            UpdateSupplierRequest request,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate supplier ID.
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

            // Step 2: Validate request.
            if (request is null ||
                string.IsNullOrWhiteSpace(request.Name) ||
                request.Name.Trim().Length > 200 ||
                request.PhoneNumber?.Length > 30 ||
                request.Email?.Length > 254)
            {
                return Result<SupplierResponse>.Failure(
                    new[]
                    {
                    new Error(
                        "Supplier.InvalidDetails",
                        "Supplier information is invalid.")
                    });
            }

            // Step 3: Retrieve the tracked supplier.
            var supplier = await _repository.GetByIdAsync(
                supplierId,
                cancellationToken);

            // Step 4: Handle missing supplier.
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

            // Step 5: Get current UTC timestamp.
            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            // Step 6: Apply changes through the Domain entity.
            supplier.UpdateDetails(
                request.Name,
                request.PhoneNumber,
                request.Email,
                utcNow);

            // Step 7: Persist changes.
            await _repository.SaveChangesAsync(
                cancellationToken);

            // Step 8: Return updated supplier information.
            var response = new SupplierResponse(
                supplier.SupplierId,
                supplier.Name,
                supplier.PhoneNumber,
                supplier.Email,
                supplier.CreatedAtUtc,
                supplier.UpdatedAtUtc);

            return Result<SupplierResponse>.Success(response);
        }
    }
}
