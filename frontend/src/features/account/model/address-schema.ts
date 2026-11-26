import { z } from 'zod';
export const addressSchema=z.object({
  recipientName:z.string().trim().min(2,'Recipient name is required'),
  contactNumber:z.string().trim().min(7,'Valid contact number is required'),
  city:z.string().trim().min(1,'City is required'),
  neighborhood:z.string().trim().min(1,'Neighborhood is required'),
  street:z.string().trim().min(1,'Street is required'),
  buildingInfo:z.string().trim(),
  additionalAddressInfo:z.string().trim(),
});
export type AddressFormValues=z.infer<typeof addressSchema>;
export const emptyAddress:AddressFormValues={recipientName:'',contactNumber:'',city:'',neighborhood:'',street:'',buildingInfo:'',additionalAddressInfo:''};
