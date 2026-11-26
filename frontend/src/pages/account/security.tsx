import { createFileRoute } from '@tanstack/react-router';
import { SecurityPage as AccountSecurityView } from '@/features/account/views/security-view';

export const Route = createFileRoute('/account/security')({ component: AccountSecurityView });
