import { useState } from 'react';
import { Link } from '@tanstack/react-router';
import { Plus } from 'lucide-react';
import { useGetProductsQuery } from '@/shared/api/generated/api.generated';
import { PageHeader } from '@/components/layout/site-layout';
import { Button } from '@/components/ui/button';
import { Card } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { EmptyState, ErrorState, LoadingState } from '@/components/ui/feedback';
import { currency } from '@/shared/lib/format';

export function OwnerProductsPage(){
  const [page,setPage]=useState(1); // Only the pagination cursor is UI state. Rows derive from RTK Query.
  const query=useGetProductsQuery({PageNumber:page,PageSize:10,SortBy:'createdAt',SortDirection:'desc'});
  return <div className="py-2"><PageHeader eyebrow="STORE MANAGEMENT" title="Products" description="Browse catalog records and open a dedicated editor for each product." action={<Button asChild><Link to="/owner/products/new"><Plus size={17}/> Create product</Link></Button>}/>
    
    <Card className="mt-7 overflow-hidden">{query.isLoading?<LoadingState/>:query.isError?<ErrorState error={query.error} onRetry={()=>void query.refetch()}/>:!query.data?.items?.length?<EmptyState title="No products found" description="Create a draft product to begin managing your electronics catalog."/>:
    <div className="overflow-x-auto"><Table className="min-w-[600px]"><TableHeader><TableRow><TableHead>Product</TableHead><TableHead>ID</TableHead><TableHead>Starting price</TableHead><TableHead className="text-right">Edit</TableHead></TableRow></TableHeader><TableBody>
      {query.data.items.map((product)=><TableRow key={product.productId}><TableCell><p className="font-semibold">{product.name||'Unnamed product'}</p><p className="mt-1 text-xs text-slate-400">{product.slug}</p></TableCell><TableCell>#{product.productId}</TableCell><TableCell>{currency(product.priceFrom)}</TableCell><TableCell className="text-right"><Button asChild size="sm" variant="outline"><Link to="/owner/products/$productId" params={{productId:String(product.productId)}}>Manage</Link></Button></TableCell></TableRow>)}
    </TableBody></Table></div>}</Card>
    {!!query.data?.items?.length&&<div className="mt-5 flex items-center justify-between"><p className="text-sm text-slate-500">Page {page} of {Math.max(1,query.data.totalPages??1)}</p><div className="flex gap-2"><Button size="sm" variant="secondary" disabled={!query.data.hasPreviousPage} onClick={()=>setPage(p=>p-1)}>Previous</Button><Button size="sm" variant="secondary" disabled={!query.data.hasNextPage} onClick={()=>setPage(p=>p+1)}>Next</Button></div></div>}
  </div>;
}
