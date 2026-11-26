using ECommerce.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.Abstractions
{
    public interface ICategoryRepository
    {
        Task<bool> ParentExistsAsync(
            long parentCategoryId,
            CancellationToken cancellationToken = default);

        Task<bool> SiblingNameExistsAsync(
            long? parentCategoryId,
            string name,
            CancellationToken cancellationToken = default);

        Task<bool> SlugExistsAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task AddAsync(//inserts a new entity.
            Category category,
            CancellationToken cancellationToken = default);

        Task<Category?> GetByIdAsync(
    long categoryId,
    CancellationToken cancellationToken = default);

        Task SaveChangesAsync(//persists modifications to an entity already tracked by EF Core.
            CancellationToken cancellationToken = default);
    }
}
