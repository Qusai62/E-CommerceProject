import { useState, type FormEvent } from 'react';
import { Save } from 'lucide-react';
import { useProductUpdateCommand } from '../hooks/use-product-editor-commands';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Field, FormError } from '@/components/ui/feedback';
import { ProductClassificationFields } from './product-classification-fields';

type EditableProduct = {
  productId: number;
  name: string;
  description: string;
  brandId: number | null;
  categoryId: number | null;
};

export function ProductInfoEditor({ product }: { product: EditableProduct }) {
  const {
    save,
    status: { error, isLoading, isSuccess },
  } = useProductUpdateCommand(product.productId);
  const [name, setName] = useState(product.name);
  const [description, setDescription] = useState(product.description);
  const [brandId, setBrandId] = useState(product.brandId?.toString() ?? '');
  const [categoryId, setCategoryId] = useState(product.categoryId?.toString() ?? '');

  const handleSave = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!name.trim()) return;
    try {
      await save({
        name: name.trim(),
        description,
        brandId: brandId ? Number(brandId) : null,
        categoryId: categoryId ? Number(categoryId) : null,
      });
    } catch {
      /* Server-side details appear in FormError. */
    }
  };

  return (
    <section>
      <h2 className="font-display text-3xl">Product details</h2>
      <p className="mt-2 text-xs text-stone-500">
        Product GET details currently lack an OpenAPI response schema; verify editor data against
        your API.
      </p>
      <form className="mt-7 space-y-5" onSubmit={(event) => void handleSave(event)}>
        <Field label="Name">
          <Input required maxLength={200} value={name} onChange={(e) => setName(e.target.value)} />
        </Field>
        <Field label="Description">
          <Textarea
            maxLength={2000}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        </Field>
        <ProductClassificationFields
          brandId={brandId}
          categoryId={categoryId}
          onBrandIdChange={setBrandId}
          onCategoryIdChange={setCategoryId}
          disabled={isLoading}
        />
        <FormError error={error} />
        {isSuccess && (
          <p role="status" className="text-sm text-moss">
            Product updated successfully.
          </p>
        )}
        <Button type="submit" disabled={isLoading || !name.trim()}>
          <Save size={17} /> {isLoading ? 'Saving…' : 'Save product'}
        </Button>
      </form>
    </section>
  );
}
