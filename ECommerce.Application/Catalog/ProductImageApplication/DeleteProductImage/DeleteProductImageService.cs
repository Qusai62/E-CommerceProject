using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.ProductImageApplication.DeleteProductImage
{
    public class DeleteProductImageService
    {
        private readonly IProductImageDeleteRepository _repository;

        public DeleteProductImageService(
            IProductImageDeleteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> ExecuteAsync(
            DeleteProductImageCommand command,
            CancellationToken cancellationToken = default)
        {
            var errors = new List<Error>();

            // 1. Validate ProductId
            if (command.ProductId <= 0)
            {
                errors.Add(new Error(
                    "Product.InvalidId",
                    "ProductId must be greater than zero.",
                    nameof(command.ProductId)));
            }

            // 2. Validate ProductImageId
            if (command.ProductImageId <= 0)
            {
                errors.Add(new Error(
                    "Image.InvalidId",
                    "ImageId must be greater than zero.",
                    nameof(command.ProductImageId)));
            }

            if (errors.Count > 0)
            {
                return Result<bool>.Failure(errors);
            }

            // 3. Find image belonging to selected product
            var image = await _repository.GetByIdAsync(
                command.ProductId,
                command.ProductImageId,
                cancellationToken);

            if (image is null)
            {
                return Result<bool>.Failure(
                    new List<Error>
                    {
                    new Error(
                        "Image.NotFound",
                        "Product image was not found.",
                        nameof(command.ProductImageId))
                    });
            }

            // 4. Delete the database record
            await _repository.DeleteAsync(
                image,
                cancellationToken);

            // 5. Return success
            return Result<bool>.Success(true);
        }
    }
}
