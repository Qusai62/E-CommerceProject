using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using TechnicalSpecificationEntity = ECommerce.Domain.Catalog.TechnicalSpecification;

namespace ECommerce.Application.Catalog.Abstractions
{
    public interface ITechnicalSpecificationRepository
    {
        Task<bool> ProductExistsAsync(
         long productId,
         CancellationToken cancellationToken = default);

        Task<bool> SpecificationNameExistsAsync(
            long productId,
            string name,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            TechnicalSpecificationEntity specification,
            CancellationToken cancellationToken = default);
    }
}
