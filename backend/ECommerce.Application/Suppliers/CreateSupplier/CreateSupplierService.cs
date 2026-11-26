using ECommerce.Domain.Supplier_Invoice___Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Suppliers.CreateSupplier
{
    public sealed class CreateSupplierService
    {
        private readonly ICreateSupplierRepository _repository;
        private readonly TimeProvider _timeProvider;

        public CreateSupplierService(
            ICreateSupplierRepository repository,
            TimeProvider timeProvider)
        {
            _repository = repository;
            _timeProvider = timeProvider;
        }

        public async Task<CreateSupplierResponse> ExecuteAsync(
            CreateSupplierRequest request,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate the request
            ArgumentNullException.ThrowIfNull(request);

            // Step 2: Get the current UTC timestamp
            var utcNow = _timeProvider.GetUtcNow().UtcDateTime;

            // Step 3: Create the domain entity
            var supplier = new Supplier(
                request.Name,
                request.PhoneNumber,
                request.Email,
                utcNow);

            // Step 4: Track the new supplier
            await _repository.AddAsync(
                supplier,
                cancellationToken);

            // Step 5: Save changes to the database
            await _repository.SaveChangesAsync(
                cancellationToken);

            // Step 6: Return the response
            return new CreateSupplierResponse(
                supplier.SupplierId,
                supplier.Name,
                supplier.PhoneNumber,
                supplier.Email,
                supplier.CreatedAtUtc);
        }
    }
}
