import { createFileRoute } from '@tanstack/react-router';
import { OwnerReportsPage } from '@/features/owner/views/reports-view';
export const Route = createFileRoute('/owner/reports')({ component: OwnerReportsPage });
