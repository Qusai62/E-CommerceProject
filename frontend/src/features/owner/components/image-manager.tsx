import { useState, type FormEvent } from 'react';
import { ImagePlus, Pencil, Trash2 } from 'lucide-react';
import { useImageCommands } from '../hooks/use-product-editor-commands';
import type { ProductImageVM } from '@/features/catalog/model/product';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { ConfirmAction } from '@/components/ui/alert-dialog';
import { Field, FormError } from '@/components/ui/feedback';
import { publicImageUrl } from '@/shared/lib/format';

export function ImageManager({
  productId,
  images,
}: {
  productId: number;
  images: ProductImageVM[];
}) {
  const commands = useImageCommands(productId);
  const [file, setFile] = useState<File | null>(null);
  const [altText, setAltText] = useState('');
  const [sortOrder, setSortOrder] = useState('0');
  const [isPrimary, setIsPrimary] = useState(false);
  const [editingId, setEditingId] = useState<number | null>(null);

  const resetForm = () => {
    setFile(null);
    setAltText('');
    setSortOrder('0');
    setIsPrimary(false);
    setEditingId(null);
  };
  const edit = (image: ProductImageVM) => {
    if (image.id === undefined) return;
    setEditingId(image.id);
    setAltText(image.altText);
    setSortOrder(String(image.sortOrder));
    setIsPrimary(image.isPrimary);
  };
  const save = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    try {
      if (editingId !== null) {
        await commands.update(editingId, { altText, sortOrder: Number(sortOrder), isPrimary });
      } else {
        if (!file) return;
        await commands.add(file, altText, Number(sortOrder));
      }
      resetForm();
    } catch {
      /* API error displayed below. */
    }
  };

  return (
    <section>
      <h2 className="font-display text-3xl">Product images</h2>
      <p className="mt-2 text-sm text-stone-500">
        Add media, update alternative text, choose the primary image or remove outdated pictures.
      </p>
      <div className="mt-6 grid gap-4 sm:grid-cols-2">
        {images.map((image, index) => (
          <div
            key={image.id ?? index}
            className="overflow-hidden rounded-xl border border-line bg-white"
          >
            <div className="grid aspect-[5/3] place-items-center bg-muted">
              {publicImageUrl(image.storageKey) ? (
                <img
                  alt={image.altText || 'Product image'}
                  src={publicImageUrl(image.storageKey)}
                  className="h-full w-full object-cover"
                  loading="lazy"
                />
              ) : (
                <ImagePlus size={28} className="text-moss" />
              )}
            </div>
            <div className="space-y-2 p-4">
              <p className="truncate text-sm font-semibold" title={image.altText}>
                {image.altText || 'Image without alternative text'}
              </p>
              <p className="text-xs text-stone-500">
                Order {image.sortOrder} · {image.isPrimary ? 'Primary' : 'Gallery image'}
              </p>
              {image.id !== undefined ? (
                <div className="flex gap-2 pt-2">
                  <Button variant="outline" size="sm" onClick={() => edit(image)}>
                    <Pencil size={13} /> Edit
                  </Button>
                  <ConfirmAction
                    destructive
                    title="Delete this image?"
                    description="The image will be removed from this product."
                    trigger={
                      <Button variant="ghost" size="sm">
                        <Trash2 size={13} /> Delete
                      </Button>
                    }
                    onConfirm={async () => {
                      try {
                        await commands.remove(image.id!);
                      } catch {
                        /* Error rendered inline. */
                      }
                    }}
                  />
                </div>
              ) : (
                <p className="text-xs text-stone-500">
                  An image ID is required for editing and deletion.
                </p>
              )}
            </div>
          </div>
        ))}
      </div>
      <form className="mt-8 space-y-5 border-t border-line pt-7" onSubmit={(e) => void save(e)}>
        <h3 className="font-semibold">
          {editingId === null ? 'Upload image' : `Edit image #${editingId}`}
        </h3>
        {editingId === null && (
          <Field
            label="Image file"
            hint="Supported formats and size limits must be enforced by the API."
          >
            <Input
              type="file"
              accept="image/*"
              onChange={(e) => setFile(e.target.files?.[0] ?? null)}
            />
          </Field>
        )}
        <Field label="Alternative text">
          <Input
            required
            maxLength={250}
            value={altText}
            onChange={(e) => setAltText(e.target.value)}
            placeholder="Describe the image for accessibility"
          />
        </Field>
        <Field label="Sort order">
          <Input
            required
            type="number"
            min="0"
            step="1"
            value={sortOrder}
            onChange={(e) => setSortOrder(e.target.value)}
          />
        </Field>
        {editingId !== null && (
          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={isPrimary}
              onChange={(e) => setIsPrimary(e.target.checked)}
            />{' '}
            Make primary image
          </label>
        )}
        <FormError error={commands.error} />
        {commands.isSuccess && (
          <p role="status" className="text-sm text-moss">
            Image changes saved.
          </p>
        )}
        <div className="flex gap-2">
          <Button
            type="submit"
            disabled={commands.isLoading || !altText.trim() || (editingId === null && !file)}
          >
            <ImagePlus size={17} /> {editingId === null ? 'Upload image' : 'Save image'}
          </Button>
          {editingId !== null && (
            <Button type="button" variant="secondary" onClick={resetForm}>
              Cancel edit
            </Button>
          )}
        </div>
      </form>
    </section>
  );
}
