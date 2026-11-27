import { useNavigate } from '@tanstack/react-router';
import { useCreateProductMutation } from '@/shared/api/generated/api.generated';
import { toCreateProductCommand, type ProductFormValues } from '../model/product-form';
export function useCreateProduct() {
  const navigate = useNavigate();
  const [create, status] = useCreateProductMutation();
  async function submit(values: ProductFormValues) {
    try {
      await create({ body: toCreateProductCommand(values) }).unwrap();
      // POST /api/products documents 201 without a response body; don't invent a new ID.
      await navigate({ to: '/owner/products' });
    } catch {
      /* RTK Query exposes the server error through status.error. */
    }
  }
  return { submit, status };
}
