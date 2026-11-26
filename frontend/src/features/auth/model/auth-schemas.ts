import { z } from 'zod';
export const loginSchema=z.object({email:z.email('Enter a valid email address'),password:z.string().min(1,'Password is required')});
export const registrationSchema=z.object({
  firstName:z.string().trim().min(1,'First name is required').max(100),
  lastName:z.string().trim().min(1,'Last name is required').max(100),
  email:z.email('Enter a valid email address'),
  phoneNumber:z.string().trim().max(25),
  password:z.string().min(8,'Use at least 8 characters').regex(/[A-Z]/,'Add uppercase').regex(/[a-z]/,'Add lowercase').regex(/\d/,'Add a number').regex(/[^a-zA-Z0-9]/,'Add a special character'),
  confirmPassword:z.string(),
}).refine((values)=>values.password===values.confirmPassword,{path:['confirmPassword'],message:'Passwords do not match'});
export type LoginFormValues=z.infer<typeof loginSchema>;
export type RegistrationFormValues=z.infer<typeof registrationSchema>;
export function registerRequest(values:RegistrationFormValues){const {confirmPassword:_,...request}=values;void _;return request;}
