using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using TechnicalSpecificationEntity = ECommerce.Domain.Catalog.TechnicalSpecification;


namespace ECommerce.Application.Catalog.Abstractions
{
    public interface ITechnicalSpecificationUpdateRepository
    {
        Task<TechnicalSpecificationEntity?> GetByIdAsync(
        long productId,
        long specificationId,
        CancellationToken cancellationToken = default);

        Task<bool> SpecificationNameExistsAsync(
            long productId,
            string name,
            long excludedSpecificationId,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
