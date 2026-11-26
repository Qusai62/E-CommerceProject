using ECommerce.Application.Categories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.ViewCategories
{
    public class ViewCategoriesService
    {
        private readonly ICategoryReadRepository _repository;

        public ViewCategoriesService(
            ICategoryReadRepository repository)
        {
            _repository = repository;
        }

        public Task<IReadOnlyList<CategoryListItemResponse>> ExecuteAsync(
            CancellationToken cancellationToken = default)
        {
            return _repository.GetAllAsync(cancellationToken);
        }
    }
}
