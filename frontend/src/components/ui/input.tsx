import * as React from 'react';
import { cn } from '@/shared/lib/cn';
export const Input = React.forwardRef<HTMLInputElement, React.InputHTMLAttributes<HTMLInputElement>>(({className, ...props},ref) => <input ref={ref} className={cn('flex h-11 w-full rounded-xl border border-line bg-white px-4 py-2 text-sm text-ink outline-none placeholder:text-stone-400 focus-visible:border-moss focus-visible:ring-2 focus-visible:ring-moss/15 disabled:bg-stone-100 disabled:opacity-60',className)} {...props}/>);
Input.displayName = 'Input';
