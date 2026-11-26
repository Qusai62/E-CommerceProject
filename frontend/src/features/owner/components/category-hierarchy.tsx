import { ChevronRight, FolderTree } from 'lucide-react';
import type { CategoryHierarchyResponse } from '@/shared/api/generated/api.generated';

function CategoryBranch({ category }: { category: CategoryHierarchyResponse }) {
  return (
    <li className="mt-2">
      <div className="flex items-center gap-2 text-sm">
        <ChevronRight size={15} className="text-stone-400" />
        <span className="font-medium">{category.name ?? 'Unnamed category'}</span>
        <span className="text-xs text-stone-500">#{category.categoryId ?? '—'}</span>
      </div>
      {category.children?.length ? (
        <ul className="ml-3 border-l border-line pl-3">
          <CategoryTree categories={category.children} />
        </ul>
      ) : null}
    </li>
  );
}

export function CategoryTree({ categories }: { categories: CategoryHierarchyResponse[] }) {
  return (
    <ul className="mt-4 space-y-1">
      {categories.map((category, index) => (
        <CategoryBranch
          key={category.categoryId ?? `${category.slug ?? category.name ?? 'category'}-${index}`}
          category={category}
        />
      ))}
    </ul>
  );
}

export function CategoryHierarchy({ categories }: { categories: CategoryHierarchyResponse[] }) {
  return (
    <section className="mt-8 border-t border-line pt-6">
      <div className="flex items-center gap-2">
        <FolderTree size={18} className="text-blue-600" />
        <h3 className="text-sm font-bold">Category hierarchy</h3>
      </div>
      <CategoryTree categories={categories} />
    </section>
  );
}
