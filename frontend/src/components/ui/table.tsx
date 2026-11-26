import type { HTMLAttributes, TableHTMLAttributes, ThHTMLAttributes, TdHTMLAttributes } from 'react';
import { cn } from '@/shared/lib/cn';
export function Table({ className, ...props }: TableHTMLAttributes<HTMLTableElement>) { return <div className="w-full overflow-x-auto"><table className={cn('w-full caption-bottom text-sm', className)} {...props}/></div>; }
export function TableHeader({ className, ...props }: HTMLAttributes<HTMLTableSectionElement>) { return <thead className={cn('bg-slate-50 text-slate-600 [&_tr]:border-b', className)} {...props}/>; }
export function TableBody({ className, ...props }: HTMLAttributes<HTMLTableSectionElement>) { return <tbody className={cn('[&_tr:last-child]:border-0', className)} {...props}/>; }
export function TableRow({ className, ...props }: HTMLAttributes<HTMLTableRowElement>) { return <tr className={cn('border-b border-line transition-colors hover:bg-slate-50/70', className)} {...props}/>; }
export function TableHead({ className, ...props }: ThHTMLAttributes<HTMLTableCellElement>) { return <th className={cn('h-12 px-4 text-left align-middle text-xs font-semibold', className)} {...props}/>; }
export function TableCell({ className, ...props }: TdHTMLAttributes<HTMLTableCellElement>) { return <td className={cn('p-4 align-middle', className)} {...props}/>; }
