/**
 * Application-facing commands for the product editor.
 * The presentation components only express user intent; transport details
 * and OpenAPI mutation arguments stay inside these hooks.
 */
import {
  useUpdateProductMutation,
  useCreateVariantMutation,
  useUpdateVariantMutation,
  useAddProductImageMutation,
  useUpdateProductImageMutation,
  useDeleteProductImageMutation,
  useAddSpecificationMutation,
  useUpdateSpecificationMutation,
  useDeleteSpecificationMutation,
  type UpdateProductRequest,
  type CreateProductVariantRequest,
  type UpdateProductVariantRequest,
  type UpdateProductImageRequest,
  type AddTechnicalSpecificationRequest,
} from '@/shared/api/generated/api.generated';

export function useProductUpdateCommand(productId: number) {
  const [mutation, status] = useUpdateProductMutation();
  return {
    save: (body: UpdateProductRequest) => mutation({ id: productId, body }).unwrap(),
    status,
  };
}

export function useVariantCommands(productId: number) {
  const [create, createStatus] = useCreateVariantMutation();
  const [update, updateStatus] = useUpdateVariantMutation();
  return {
    create: (body: CreateProductVariantRequest) => create({ productId, body }).unwrap(),
    update: (variantId: number, body: UpdateProductVariantRequest) => update({ productId, variantId, body }).unwrap(),
    error: createStatus.error ?? updateStatus.error,
    isLoading: createStatus.isLoading || updateStatus.isLoading,
    isSuccess: createStatus.isSuccess || updateStatus.isSuccess,
  };
}

export function useImageCommands(productId: number) {
  const [add, addStatus] = useAddProductImageMutation();
  const [update, updateStatus] = useUpdateProductImageMutation();
  const [remove, removeStatus] = useDeleteProductImageMutation();
  return {
    add: (file: File, altText: string, sortOrder: number) => {
      const body = new FormData();
      body.append('File', file);
      body.append('AltText', altText);
      body.append('SortOrder', String(sortOrder));
      return add({ productId, body }).unwrap();
    },
    update: (imageId: number, body: UpdateProductImageRequest) => update({ productId, imageId, body }).unwrap(),
    remove: (imageId: number) => remove({ productId, imageId }).unwrap(),
    error: addStatus.error ?? updateStatus.error ?? removeStatus.error,
    isLoading: addStatus.isLoading || updateStatus.isLoading || removeStatus.isLoading,
    isSuccess: addStatus.isSuccess || updateStatus.isSuccess || removeStatus.isSuccess,
  };
}

export function useSpecificationCommands(productId: number) {
  const [add, addStatus] = useAddSpecificationMutation();
  const [update, updateStatus] = useUpdateSpecificationMutation();
  const [remove, removeStatus] = useDeleteSpecificationMutation();
  return {
    add: (body: AddTechnicalSpecificationRequest) => add({ productId, body }).unwrap(),
    update: (specificationId: number, body: AddTechnicalSpecificationRequest) => update({ productId, specificationId, body }).unwrap(),
    remove: (specificationId: number) => remove({ productId, specificationId }).unwrap(),
    error: addStatus.error ?? updateStatus.error ?? removeStatus.error,
    isLoading: addStatus.isLoading || updateStatus.isLoading || removeStatus.isLoading,
    isSuccess: addStatus.isSuccess || updateStatus.isSuccess || removeStatus.isSuccess,
  };
}
