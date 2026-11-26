import { z } from 'zod';
export const productFormSchema=z.object({
  name:z.string().trim().min(1,'Product name is required').max(200),
  description:z.string().max(2000),
  brandId:z.string(),
  categoryId:z.string(),
});
export type ProductFormValues=z.infer<typeof productFormSchema>;
export const productFormDefaults:ProductFormValues={name:'',description:'',brandId:'',categoryId:''};
export function toCreateProductCommand(input:ProductFormValues){return {
  name:input.name.trim(),description:input.description.trim(),
  brandId:input.brandId?Number(input.brandId):null,
  categoryId:input.categoryId?Number(input.categoryId):null,
};}
