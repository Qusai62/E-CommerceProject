import { createFileRoute } from '@tanstack/react-router';
import { HomePage as HomeView } from '@/features/home/views/home-view';

export const Route = createFileRoute('/')({ component: HomeView });
