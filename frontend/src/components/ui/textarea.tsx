import * as React from 'react';
import { cn } from '@/shared/lib/cn';
export const Textarea = React.forwardRef<
  HTMLTextAreaElement,
  React.TextareaHTMLAttributes<HTMLTextAreaElement>
>(({ className, ...props }, ref) => (
  <textarea
    ref={ref}
    className={cn(
      'flex min-h-24 w-full resize-y rounded-xl border border-line bg-white px-4 py-3 text-sm outline-none focus-visible:border-moss focus-visible:ring-2 focus-visible:ring-moss/15',
      className,
    )}
    {...props}
  />
));
Textarea.displayName = 'Textarea';
