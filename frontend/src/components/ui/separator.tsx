import * as React from 'react';
import * as Primitive from '@radix-ui/react-separator';
import { cn } from '@/shared/lib/cn';
export const Separator = React.forwardRef<React.ElementRef<typeof Primitive.Root>, React.ComponentPropsWithoutRef<typeof Primitive.Root>>(
  ({ className, orientation = 'horizontal', decorative = true, ...props }, ref) => <Primitive.Root ref={ref} decorative={decorative} orientation={orientation} className={cn('shrink-0 bg-line', orientation === 'horizontal' ? 'h-px w-full' : 'h-full w-px', className)} {...props} />,
);
Separator.displayName = Primitive.Root.displayName;
