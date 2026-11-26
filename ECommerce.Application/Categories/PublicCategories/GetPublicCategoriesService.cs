using ECommerce.Application.Categories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.PublicCategories
{
    public sealed class GetPublicCategoriesService
    {
        private readonly ICategoryReadRepository _repository;

        public GetPublicCategoriesService(
            ICategoryReadRepository repository)
        {
            _repository = repository;
        }

        public Task<IReadOnlyList<PublicCategoryResponse>> ExecuteAsync(
            CancellationToken cancellationToken = default)
        {
            return _repository.GetPublicAsync(cancellationToken);
        }
    }
}
