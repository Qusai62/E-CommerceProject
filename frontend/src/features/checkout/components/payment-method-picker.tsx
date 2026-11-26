import { Banknote } from 'lucide-react';
import type { PaymentMethod } from '@/shared/api/generated/api.generated';
import { cn } from '@/shared/lib/cn';
import { paymentOptions } from '../model/checkout';

export function PaymentMethodPicker({ value, onChange }: { value: PaymentMethod; onChange: (method: PaymentMethod) => void }) {
  return (
    <fieldset className="space-y-3">
      <legend className="mb-4 text-sm font-bold">How would you like to pay?</legend>
      {paymentOptions.map((option) => {
        return (
          <label key={option.value} className={cn('flex cursor-pointer items-start gap-3 rounded-2xl border p-4 transition-colors focus-within:ring-2 focus-within:ring-blue-500', option.value === value ? 'border-blue-600 bg-blue-50/60' : 'border-line bg-white hover:border-blue-200')}>
            <input type="radio" name="payment-method" value={option.value} checked={option.value === value} onChange={() => onChange(option.value)} className="mt-1 size-4 accent-blue-600" />
            <Banknote size={20} className="mt-0.5 shrink-0 text-blue-700" />
            <span className="min-w-0"><span className="block text-sm font-bold">{option.label}</span><span className="mt-1 block text-xs leading-5 text-slate-600">{option.description}</span></span>
          </label>
        );
      })}
    </fieldset>
  );
}
