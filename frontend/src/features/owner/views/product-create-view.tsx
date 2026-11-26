import { Link } from '@tanstack/react-router';
import { PageHeader, BackLink } from '@/components/layout/site-layout';
import { Card } from '@/components/ui/card';
import { FormError } from '@/components/ui/feedback';
import { ProductForm } from '../components/product-form';
import { useCreateProduct } from '../hooks/use-create-product';

export function ProductCreateView(){
  const {submit,status}=useCreateProduct();
  return <div className="page-container max-w-4xl py-12"><BackLink to="/owner/products">Back to products</BackLink><div className="mt-6"><PageHeader eyebrow="NEW PRODUCT" title="Create a product draft" description="Set the product identity first. After saving, open the product to manage variants, specifications and images."/></div><Card className="mt-8 p-6 sm:p-9"><ProductForm onSave={submit} submitting={status.isLoading} error={<FormError error={status.error}/>} /></Card><p className="mt-6 text-sm text-slate-500">Product creation uses <strong>POST /api/products</strong>. New products must be published according to your API business rules. <Link to="/owner/products" className="text-moss underline">Return to the catalog</Link></p></div>;
}
