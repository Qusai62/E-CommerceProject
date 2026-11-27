import * as React from 'react';
import { cn } from '@/shared/lib/cn';
export const Card = React.forwardRef<HTMLDivElement, React.HTMLAttributes<HTMLDivElement>>(
  ({ className, ...props }, ref) => (
    <div
      ref={ref}
      data-slot="card"
      className={cn(
        'rounded-2xl border border-line bg-white shadow-[0_4px_20px_-12px_#14274642]',
        className,
      )}
      {...props}
    />
  ),
);
Card.displayName = 'Card';
export function CardHeader(props: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div
      data-slot="card-header"
      {...props}
      className={cn('flex flex-col gap-2 p-6', props.className)}
    />
  );
}
export function CardTitle(props: React.HTMLAttributes<HTMLHeadingElement>) {
  return (
    <h3
      data-slot="card-title"
      {...props}
      className={cn('text-lg font-semibold tracking-tight', props.className)}
    />
  );
}
export function CardDescription(props: React.HTMLAttributes<HTMLParagraphElement>) {
  return (
    <p
      data-slot="card-description"
      {...props}
      className={cn('text-sm text-slate-500', props.className)}
    />
  );
}
export function CardContent(props: React.HTMLAttributes<HTMLDivElement>) {
  return <div data-slot="card-content" {...props} className={cn('p-6 pt-0', props.className)} />;
}
export function CardFooter(props: React.HTMLAttributes<HTMLDivElement>) {
  return (
    <div
      data-slot="card-footer"
      {...props}
      className={cn('flex items-center p-6 pt-0', props.className)}
    />
  );
}
