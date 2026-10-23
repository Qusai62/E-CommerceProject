using ECommerce.Application.Catalog.Abstractions;
using ECommerce.Application.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Catalog.TechnicalSpecification.DeleteTechnicalSpecification
{
    public class DeleteTechnicalSpecificationService
    {
        private readonly ITechnicalSpecificationDeleteRepository _repository;

        public DeleteTechnicalSpecificationService(
            ITechnicalSpecificationDeleteRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<bool>> ExecuteAsync(
            DeleteTechnicalSpecificationCommand command,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(command);

            var errors = new List<Error>();

            if (command.ProductId <= 0)
            {
                errors.Add(new Error(
                    "Product.InvalidId",
                    "ProductId must be positive.",
                    "ProductId"));
            }

            if (command.SpecificationId <= 0)
            {
                errors.Add(new Error(
                    "Specification.InvalidId",
                    "SpecificationId must be positive.",
                    "SpecificationId"));
            }

            if (errors.Count > 0)
            {
                return Result<bool>.Failure(errors);
            }

            var deleted = await _repository.DeleteAsync(
                command.ProductId,
                command.SpecificationId,
                cancellationToken);

            if (!deleted)
            {
                return Result<bool>.Failure(
                    new[]
                    {
                    new Error(
                        "Specification.NotFound",
                        "The specification does not exist for this product.",
                        "SpecificationId")
                    });
            }

            return Result<bool>.Success(true);
        }
    }
}

