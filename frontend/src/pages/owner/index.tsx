import { createFileRoute } from '@tanstack/react-router';
import { OwnerDashboard } from '@/features/owner/views/dashboard-view';
export const Route = createFileRoute('/owner/')({ component: OwnerDashboard });
