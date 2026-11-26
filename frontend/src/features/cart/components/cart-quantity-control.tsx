import { Minus, Plus } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { nextCartQuantity } from '../model/quantity';

type CartQuantityControlProps = {
  quantity: number;
  itemName: string;
  isUpdating: boolean;
  disabled?: boolean;
  onQuantityChange: (quantity: number) => void;
};

/** Pure presentation: no RTK Query, no side effects, no duplicated quantity state. */
export function CartQuantityControl({
  quantity,
  itemName,
  isUpdating,
  disabled = false,
  onQuantityChange,
}: CartQuantityControlProps) {
  const smaller = nextCartQuantity(quantity, -1);
  const larger = nextCartQuantity(quantity, 1);
  const unableToUpdate = disabled || isUpdating;

  return (
    <div role="group" aria-label={`Quantity for ${itemName}`} className="inline-flex items-center gap-1 rounded-xl border border-line bg-white p-1">
      <Button
        type="button"
        variant="ghost"
        size="icon"
        className="h-8 w-8"
        aria-label={`Decrease ${itemName} quantity`}
        disabled={unableToUpdate || smaller === null}
        onClick={() => { if (smaller !== null) onQuantityChange(smaller); }}
      >
        <Minus aria-hidden="true" size={15} />
      </Button>
      <output className="min-w-9 text-center text-sm font-semibold tabular-nums" aria-label="Current quantity">
        {quantity}
      </output>
      <Button
        type="button"
        variant="ghost"
        size="icon"
        className="h-8 w-8"
        aria-label={`Increase ${itemName} quantity`}
        disabled={unableToUpdate || larger === null}
        onClick={() => { if (larger !== null) onQuantityChange(larger); }}
      >
        <Plus aria-hidden="true" size={15} />
      </Button>
    </div>
  );
}
