import { createFileRoute } from '@tanstack/react-router';
import { OwnerCategoriesPage } from '@/features/owner/views/categories-view';
export const Route = createFileRoute('/owner/categories')({ component: OwnerCategoriesPage });
