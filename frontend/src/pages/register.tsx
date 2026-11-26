import { createFileRoute } from '@tanstack/react-router';
import { RegisterPage as RegisterView } from '@/features/auth/views/register-view';

export const Route = createFileRoute('/register')({ component: RegisterView });
