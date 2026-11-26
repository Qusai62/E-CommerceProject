import { Link } from '@tanstack/react-router';
import { MapPin, Plus } from 'lucide-react';
import type { CustomerAddressResponse } from '@/shared/api/generated/api.generated';
import { cn } from '@/shared/lib/cn';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { hasValidAddressId } from '../model/checkout';

export interface AddressPickerProps {
  addresses: CustomerAddressResponse[];
  selectedId: number | undefined;
  onSelect: (id: number) => void;
}
export function AddressPicker({ addresses, selectedId, onSelect }: AddressPickerProps) {
  return (
    <fieldset className="space-y-3">
      <legend className="mb-4 text-sm font-bold">Saved delivery address</legend>
      {!addresses.length ? (
        <div className="rounded-2xl border border-dashed border-line bg-slate-50 p-5 text-sm text-slate-600">
          No delivery address is saved yet. Add one to continue.
        </div>
      ) : (
        <div className="grid gap-3 sm:grid-cols-2">
          {addresses.filter(hasValidAddressId).map((address) => (
            <label key={address.savedAddressId} className={cn('flex cursor-pointer gap-3 rounded-2xl border p-4 transition-colors focus-within:ring-2 focus-within:ring-blue-500', address.savedAddressId === selectedId ? 'border-blue-600 bg-blue-50/60' : 'border-line bg-white hover:border-blue-200')}>
              <input name="delivery-address" type="radio" value={address.savedAddressId} checked={selectedId === address.savedAddressId} onChange={() => onSelect(address.savedAddressId)} className="mt-1 size-4 accent-blue-600" />
              <span className="min-w-0 flex-1">
                <span className="flex flex-wrap items-center gap-2 text-sm font-bold"><MapPin size={15} className="text-blue-600" />{address.recipientName || 'Delivery address'}{address.isDefault && <Badge>Default</Badge>}</span>
                <span className="mt-2 block text-xs leading-6 text-slate-600">{[address.buildingInfo, address.street, address.neighborhood, address.city].filter(Boolean).join(', ')}</span>
                <span className="mt-1 block text-xs text-slate-500">{address.contactNumber}</span>
              </span>
            </label>
          ))}
        </div>
      )}
      <Button variant="outline" size="sm" asChild><Link to="/account/addresses/new" search={{returnTo: '/checkout'}}><Plus size={15} /> Add another address</Link></Button>
    </fieldset>
  );
}
