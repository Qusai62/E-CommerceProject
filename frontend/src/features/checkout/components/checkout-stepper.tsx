import { Check } from 'lucide-react';
import { cn } from '@/shared/lib/cn';

const steps = ['Cart', 'Delivery & payment', 'Review'] as const;
export function CheckoutStepper({ current }: { current: 1 | 2 | 3 }) {
  return (
    <ol aria-label="Checkout progress" className="flex items-start gap-2 sm:gap-4">
      {steps.map((step, index) => {
        const number = (index + 1) as 1 | 2 | 3;
        return (
          <li
            key={step}
            aria-current={current === number ? 'step' : undefined}
            className="flex min-w-0 flex-1 items-start gap-2 sm:gap-3"
          >
            <span
              className={cn(
                'grid size-8 shrink-0 place-items-center rounded-full border text-xs font-bold sm:size-9',
                current === number
                  ? 'border-blue-700 bg-blue-700 text-white'
                  : current > number
                    ? 'border-emerald-600 bg-emerald-50 text-emerald-700'
                    : 'border-slate-200 bg-white text-slate-500',
              )}
            >
              {number < current ? <Check size={15} /> : number}
            </span>
            <span
              className={cn(
                'min-w-0 pt-1.5 text-[11px] font-semibold leading-4 sm:text-sm',
                current === number ? 'text-ink' : 'text-slate-500',
              )}
            >
              {step}
            </span>
          </li>
        );
      })}
    </ol>
  );
}
