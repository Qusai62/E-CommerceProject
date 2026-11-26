import { createFileRoute } from '@tanstack/react-router';
import { AccountPage as AccountProfileView } from '@/features/account/views/profile-view';

export const Route = createFileRoute('/account/')({ component: AccountProfileView });
