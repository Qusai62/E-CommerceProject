using ECommerce.Application.Categories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Categories.ViewHierarchy
{
    public class GetCategoryHierarchyService
    {
        private readonly ICategoryReadRepository _repository;

        public GetCategoryHierarchyService(
            ICategoryReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<CategoryHierarchyResponse>> ExecuteAsync(
            CancellationToken cancellationToken = default)
        {
            // 1. Load flat category records.
            var categories = await _repository.GetHierarchyRowsAsync(
                cancellationToken);

            // 2. Create one response node per category.
            var nodes = categories.ToDictionary(
                category => category.CategoryId,
                category => new CategoryHierarchyResponse
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    ParentCategoryId = category.ParentCategoryId,
                    IsActive = category.IsActive,
                    SortOrder = category.SortOrder
                });

            // 3. Store the root nodes.
            var roots = new List<CategoryHierarchyResponse>();

            // 4. Connect every category to its parent.
            foreach (var category in categories)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var node = nodes[category.CategoryId];

                if (category.ParentCategoryId is null)
                {
                    roots.Add(node);
                    continue;
                }

                if (!nodes.TryGetValue(
                    category.ParentCategoryId.Value,
                    out var parent))
                {
                    throw new InvalidOperationException(
                        "Category hierarchy contains a missing parent.");
                }

                parent.Children.Add(node);
            }

            // 5. Verify that all nodes are reachable from roots.
            // This detects disconnected cycles and invalid hierarchies.
            var visited = new HashSet<long>();
            var stack = new Stack<CategoryHierarchyResponse>(roots);

            while (stack.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var node = stack.Pop();

                if (!visited.Add(node.CategoryId))
                {
                    throw new InvalidOperationException(
                        "Category hierarchy contains a cycle or duplicate.");
                }

                foreach (var child in node.Children)
                {
                    stack.Push(child);
                }
            }

            if (visited.Count != categories.Count)
            {
                throw new InvalidOperationException(
                    "Category hierarchy contains unreachable categories.");
            }

            // 6. Return the tree.
            return roots;
        }
    }
}
