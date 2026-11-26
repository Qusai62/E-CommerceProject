import { createFileRoute } from '@tanstack/react-router';
import { OwnerBrandsPage } from '@/features/owner/views/brands-view';

export const Route = createFileRoute('/owner/brands')({ component: OwnerBrandsPage });
