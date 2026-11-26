using ECommerce.Application.Categories.PublicCategories;
using ECommerce.Application.Categories.ViewCategories;
using ECommerce.Application.Categories.ViewHierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.Abstractions
{
    public interface ICategoryReadRepository
    {
        Task<IReadOnlyList<CategoryListItemResponse>> GetAllAsync(
            CancellationToken cancellationToken = default);

        // Customer browsing
        Task<IReadOnlyList<PublicCategoryResponse>> GetPublicAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<CategoryHierarchyRow>> GetHierarchyRowsAsync(
    CancellationToken cancellationToken = default);
    }
}
