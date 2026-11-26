using System;
using System.Collections.Generic;
using ProductEntity = ECommerce.Domain.Catalog.Product;


namespace ECommerce.Application.Catalog.Common_repository_contract
{
    public interface IProductActivationRepository
    {
        Task<ProductEntity?> GetForUpdateAsync(
        long productId,
        CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);

        Task<bool> HasValidCatalogReferencesAsync(
    long productId,
    CancellationToken cancellationToken = default);
    }
}
