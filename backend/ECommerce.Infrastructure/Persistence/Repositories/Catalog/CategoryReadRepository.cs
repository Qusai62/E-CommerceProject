using ECommerce.Application.Categories.Abstractions;
using ECommerce.Application.Categories.PublicCategories;
using ECommerce.Application.Categories.ViewCategories;
using ECommerce.Application.Categories.ViewHierarchy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Catalog
{
    public class CategoryReadRepository: ICategoryReadRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryReadRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<CategoryListItemResponse>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.SortOrder)
                .ThenBy(category => category.Name)
                .ThenBy(category => category.CategoryId)
                .Select(category => new CategoryListItemResponse(
                    category.CategoryId,
                    category.Name,
                    category.Slug,
                    category.Description,
                    category.ParentCategoryId,
                    category.IsActive,
                    category.SortOrder,
                    category.Products.Count(),
                    category.CreatedAtUtc,
                    category.UpdatedAtUtc
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<PublicCategoryResponse>> GetPublicAsync(
    CancellationToken cancellationToken = default)
        {
            // Load the category information needed for public visibility.
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(category => new
                {
                    category.CategoryId,
                    category.Name,
                    category.Slug,
                    category.ParentCategoryId,
                    category.IsActive,
                    category.SortOrder
                })
                .ToListAsync(cancellationToken);

            // Lookup every category, including inactive ancestors.
            var byId = categories.ToDictionary(
                category => category.CategoryId);

            // A category is visible only if it and every ancestor are active.
            bool IsVisible(long categoryId)
            {
                var visited = new HashSet<long>();
                var currentId = (long?)categoryId;

                while (currentId.HasValue)
                {
                    if (!visited.Add(currentId.Value))
                    {
                        // A hierarchy cycle is invalid; hide the category.
                        return false;
                    }

                    if (!byId.TryGetValue(currentId.Value, out var current))
                    {
                        // A missing ancestor makes the hierarchy invalid.
                        return false;
                    }

                    if (!current.IsActive)
                    {
                        return false;
                    }

                    currentId = current.ParentCategoryId;
                }

                return true;
            }

            return categories
                .Where(category => IsVisible(category.CategoryId))
                .OrderBy(category => category.SortOrder)
                .ThenBy(category => category.Name)
                .ThenBy(category => category.CategoryId)
                .Select(category => new PublicCategoryResponse(
                    category.CategoryId,
                    category.Name,
                    category.Slug,
                    category.ParentCategoryId,
                    category.SortOrder))
                .ToList();
        }
        public async Task<IReadOnlyList<CategoryHierarchyRow>> GetHierarchyRowsAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(category => category.SortOrder)
                .ThenBy(category => category.Name)
                .ThenBy(category => category.CategoryId)
                .Select(category => new CategoryHierarchyRow(
                    category.CategoryId,
                    category.Name,
                    category.ParentCategoryId,
                    category.IsActive,
                    category.SortOrder
                ))
                .ToListAsync(cancellationToken);
        }

    }
}
