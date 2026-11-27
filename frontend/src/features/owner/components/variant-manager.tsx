import { useState, type FormEvent } from 'react';
import { Plus, Trash2 } from 'lucide-react';
import { useVariantCommands } from '../hooks/use-product-editor-commands';
import type { ProductVariantVM } from '@/features/catalog/model/product';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Field, FormError } from '@/components/ui/feedback';
import { currency } from '@/shared/lib/format';

type AttributeDraft = { name: string; value: string };
export function VariantManager({
  productId,
  variants,
}: {
  productId: number;
  variants: ProductVariantVM[];
}) {
  const commands = useVariantCommands(productId);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [sku, setSku] = useState('');
  const [price, setPrice] = useState('');
  const [isActive, setIsActive] = useState(true);
  const [attributes, setAttributes] = useState<AttributeDraft[]>([]);

  const clear = () => {
    setEditingId(null);
    setSku('');
    setPrice('');
    setIsActive(true);
    setAttributes([]);
  };
  const edit = (variant: ProductVariantVM) => {
    setEditingId(variant.id);
    setSku(variant.sku);
    setPrice(String(variant.price));
    setIsActive(variant.isActive);
    setAttributes([]);
  };
  const save = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!sku.trim() || Number(price) <= 0) return;
    try {
      if (editingId !== null) {
        await commands.update(editingId, { sku: sku.trim(), price: Number(price), isActive });
      } else {
        await commands.create({
          sku: sku.trim(),
          price: Number(price),
          attributes: attributes
            .filter((a) => a.name.trim() && a.value.trim())
            .map((a, index) => ({
              name: a.name.trim(),
              value: a.value.trim(),
              sortOrder: index,
            })),
        });
      }
      clear();
    } catch {
      /* Error is rendered below. */
    }
  };

  return (
    <section>
      <h2 className="font-display text-3xl">Variants & pricing</h2>
      <p className="mt-2 text-sm text-stone-500">
        Add a SKU for each sellable option. New variants start with zero stock.
      </p>
      <div className="mt-6 space-y-3">
        {variants.map((variant) => (
          <div
            key={variant.id}
            className="flex flex-wrap items-center justify-between gap-3 rounded-xl border border-line p-4"
          >
            <div>
              <p className="text-sm font-semibold">{variant.sku}</p>
              <p className="mt-1 text-xs text-stone-500">
                {currency(variant.price)} · {variant.isActive ? 'Active' : 'Inactive'} · #
                {variant.id}
              </p>
            </div>
            <Button variant="outline" size="sm" onClick={() => edit(variant)}>
              Edit variant
            </Button>
          </div>
        ))}
        {!variants.length && (
          <p className="rounded-lg bg-muted p-4 text-sm text-stone-500">
            No variants returned by the product details API.
          </p>
        )}
      </div>
      <form className="mt-8 space-y-5 border-t border-line pt-7" onSubmit={(e) => void save(e)}>
        <h3 className="font-semibold">{editingId !== null ? 'Edit variant' : 'Create variant'}</h3>
        <div className="grid gap-4 sm:grid-cols-2">
          <Field label="SKU">
            <Input required maxLength={100} value={sku} onChange={(e) => setSku(e.target.value)} />
          </Field>
          <Field label="Price">
            <Input
              required
              type="number"
              step="0.001"
              min="0.001"
              value={price}
              onChange={(e) => setPrice(e.target.value)}
            />
          </Field>
        </div>
        {editingId !== null ? (
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={isActive}
              onChange={(e) => setIsActive(e.target.checked)}
            />{' '}
            Active variant
          </label>
        ) : (
          <div>
            <p className="mb-3 text-xs font-semibold">Optional attributes (color, size, etc.)</p>
            <div className="space-y-3">
              {attributes.map((attribute, index) => (
                <div key={index} className="flex items-center gap-2">
                  <Input
                    aria-label={`Attribute ${index + 1} name`}
                    placeholder="Name"
                    value={attribute.name}
                    onChange={(e) =>
                      setAttributes((old) =>
                        old.map((row, i) => (i === index ? { ...row, name: e.target.value } : row)),
                      )
                    }
                  />
                  <Input
                    aria-label={`Attribute ${index + 1} value`}
                    placeholder="Value"
                    value={attribute.value}
                    onChange={(e) =>
                      setAttributes((old) =>
                        old.map((row, i) =>
                          i === index ? { ...row, value: e.target.value } : row,
                        ),
                      )
                    }
                  />
                  <Button
                    type="button"
                    size="icon"
                    variant="ghost"
                    aria-label={`Remove attribute ${index + 1}`}
                    onClick={() => setAttributes((old) => old.filter((_, i) => i !== index))}
                  >
                    <Trash2 size={16} />
                  </Button>
                </div>
              ))}
            </div>
            <Button
              className="mt-3"
              size="sm"
              type="button"
              variant="outline"
              onClick={() => setAttributes((old) => [...old, { name: '', value: '' }])}
            >
              <Plus size={14} /> Add attribute
            </Button>
          </div>
        )}
        <FormError error={commands.error} />
        {commands.isSuccess && (
          <p role="status" className="text-sm text-moss">
            Variant saved.
          </p>
        )}
        <div className="flex gap-2">
          <Button
            type="submit"
            disabled={commands.isLoading || !sku.trim() || !(Number(price) > 0)}
          >
            {editingId === null ? 'Add variant' : 'Save variant'}
          </Button>
          {editingId !== null && (
            <Button variant="secondary" type="button" onClick={clear}>
              Cancel
            </Button>
          )}
        </div>
      </form>
    </section>
  );
}
