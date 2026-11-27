import type { ReactNode } from 'react';
import { AlertCircle, Inbox, LoaderCircle } from 'lucide-react';
import { Button } from './button';
import { getErrorMessage } from '@/shared/lib/error';
export function LoadingState({ message = 'Loading your space…' }: { message?: string }) {
  return (
    <div
      role="status"
      className="flex min-h-56 flex-col items-center justify-center gap-3 text-stone-500"
    >
      <LoaderCircle className="animate-spin" size={25} />
      <span className="text-sm">{message}</span>
    </div>
  );
}
export function ErrorState({ error, onRetry }: { error: unknown; onRetry?: () => void }) {
  return (
    <div
      role="alert"
      className="mx-auto flex max-w-md flex-col items-center gap-3 rounded-2xl border border-red-100 bg-red-50 px-6 py-10 text-center"
    >
      <AlertCircle className="text-red-600" />
      <h3 className="font-semibold">We couldn’t load this right now</h3>
      <p className="text-sm text-stone-600">{getErrorMessage(error)}</p>
      {onRetry && (
        <Button variant="secondary" onClick={onRetry}>
          Try again
        </Button>
      )}
    </div>
  );
}
export function EmptyState({
  icon,
  title,
  description,
  action,
}: {
  icon?: ReactNode;
  title: string;
  description: string;
  action?: ReactNode;
}) {
  return (
    <div className="mx-auto flex min-h-64 max-w-md flex-col items-center justify-center gap-3 text-center">
      <div className="rounded-full bg-stone-100 p-5 text-stone-500">
        {icon ?? <Inbox size={30} />}
      </div>
      <h3 className="font-display text-2xl text-ink">{title}</h3>
      <p className="text-sm leading-6 text-stone-500">{description}</p>
      {action && <div className="mt-2">{action}</div>}
    </div>
  );
}
export function FormError({ error }: { error: unknown }) {
  return error ? (
    <p role="alert" className="rounded-xl bg-red-50 p-3 text-sm text-red-700">
      {getErrorMessage(error)}
    </p>
  ) : null;
}
export function Field({
  label,
  error,
  children,
  hint,
}: {
  label: string;
  error?: string;
  children: ReactNode;
  hint?: string;
}) {
  return (
    <label className="block space-y-1.5">
      <span className="block text-xs font-semibold tracking-wide text-stone-700">{label}</span>
      {children}
      {hint && <span className="block text-xs text-stone-500">{hint}</span>}
      {error && <span className="block text-xs text-red-600">{error}</span>}
    </label>
  );
}
