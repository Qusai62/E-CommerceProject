using ECommerce.Application.Categories.Abstractions;
using ECommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories.Catalog
{
    public sealed class CategoryRepository: ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ParentExistsAsync(
            long parentCategoryId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(
                    category =>
                        category.CategoryId == parentCategoryId,
                    cancellationToken);
        }

        public async Task<bool> SiblingNameExistsAsync(
            long? parentCategoryId,
            string name,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(
                    category =>
                        category.ParentCategoryId == parentCategoryId &&
                        category.Name == name,
                    cancellationToken);
        }

        public async Task<bool> SlugExistsAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(
                    category => category.Slug == slug,
                    cancellationToken);
        }

        public async Task AddAsync(
            Category category,
            CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(
                category,
                cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Category?> GetByIdAsync(
    long categoryId,
    CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(
                    category => category.CategoryId == categoryId,
                    cancellationToken);
        }

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
