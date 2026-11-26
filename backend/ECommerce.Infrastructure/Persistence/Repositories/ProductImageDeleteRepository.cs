using ECommerce.Application.Catalog.ProductImageApplication.DeleteProductImage;
using ECommerce.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Persistence.Repositories
{
    public class ProductImageDeleteRepository: IProductImageDeleteRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductImageDeleteRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<ProductImage?> GetByIdAsync(
            long productId,
            long imageId,
            CancellationToken cancellationToken = default)
        {
            return _context.ProductImages
                .FirstOrDefaultAsync(
                    image => image.ProductId == productId
                        && image.ProductImageId == imageId,
                    cancellationToken);
        }

        public async Task DeleteAsync(
            ProductImage image,
            CancellationToken cancellationToken = default)
        {
            _context.ProductImages.Remove(image);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
