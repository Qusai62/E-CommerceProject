import type { ReactNode } from 'react';
import { Link } from '@tanstack/react-router';
import { useGetProductByIdQuery } from '@/shared/api/generated/api.generated';
import { toProductDetail, type ProductDetailVM } from '@/features/catalog/model/product';
import { BackLink, PageHeader } from '@/components/layout/site-layout';
import { Card } from '@/components/ui/card';
import { ErrorState, LoadingState } from '@/components/ui/feedback';

export interface ProductResource { product:ProductDetailVM; raw:Record<string,unknown> }
interface ProductResourceLayoutProps {
  productId:number;
  title:string;
  children:(resource:ProductResource)=>ReactNode;
}
const links=[
 {to:'/owner/products/$productId',label:'Overview'},
 {to:'/owner/products/$productId/edit',label:'Details'},
 {to:'/owner/products/$productId/variants',label:'Variants'},
 {to:'/owner/products/$productId/images',label:'Images'},
 {to:'/owner/products/$productId/specifications',label:'Specifications'},
] as const;
export function ProductResourceLayout({productId,title,children}:ProductResourceLayoutProps){
  const valid=Number.isSafeInteger(productId)&&productId>0;
  const query=useGetProductByIdQuery({id:productId},{skip:!valid});
  const product=toProductDetail(query.data);
  if(!valid)return <ErrorState error={new Error('Invalid product identifier')}/>;
  if(query.isLoading)return <LoadingState/>;
  if(query.isError)return <ErrorState error={query.error} onRetry={()=>void query.refetch()}/>;
  if(!product)return <ErrorState error={new Error('The API returned no usable product details. Publish a GET /api/products/{id} response schema.')}/>;
  const raw=query.data&&typeof query.data==='object'?query.data as Record<string,unknown>:{};
  return <div className="page-container py-12"><BackLink to="/owner/products">Back to catalog</BackLink><div className="mt-6"><PageHeader eyebrow={`PRODUCT #${productId}`} title={title} description={`${product.name} · Manage product details and inventory integrations.`}/></div>
  <nav aria-label="Product editor" className="mt-7 flex flex-wrap gap-2">{links.map(({to,label})=><Link key={to} to={to} params={{productId:String(productId)}} className="rounded-xl border border-line bg-white px-4 py-2 text-sm font-medium text-slate-600 hover:border-blue-300 hover:text-moss" activeProps={{className:'!border-moss !bg-blue-50 !text-blue-700'}} activeOptions={{exact:true}}>{label}</Link>)}</nav>
  <Card className="mt-6 p-6 sm:p-9">{children({product,raw})}</Card>
  <p className="mt-5 text-xs text-slate-500">The Swagger product-detail response is currently undocumented; editing existing image/spec records requires IDs returned by the actual API.</p>
  </div>;
}
