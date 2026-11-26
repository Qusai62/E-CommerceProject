import { useMemo } from 'react';
import { type ColumnDef, flexRender, getCoreRowModel, useReactTable } from '@tanstack/react-table';
import { Plus } from 'lucide-react';
import type { StoreOwnerStockResponse } from '@/shared/api/generated/api.generated';
import { Card } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableHead, TableHeader, TableRow, TableCell } from '@/components/ui/table';

interface InventoryTableProps{rows:StoreOwnerStockResponse[];onAdjust:(item:StoreOwnerStockResponse)=>void;}
export function InventoryTable({rows,onAdjust}:InventoryTableProps){
  const columns=useMemo<ColumnDef<StoreOwnerStockResponse>[]>(()=>[
    {accessorKey:'productName',header:'Product',cell:({row})=><div><b>{row.original.productName||'Untitled'}</b><p className="mt-1 text-xs text-slate-400">{row.original.sku}</p></div>},
    {accessorKey:'onHandQuantity',header:'On hand'},
    {accessorKey:'reservedQuantity',header:'Reserved'},
    {accessorKey:'securedCODQuantity',header:'COD locked'},
    {accessorKey:'availableQuantity',header:'Available',cell:({getValue})=><b>{getValue<number>()??0}</b>},
    {accessorKey:'isInStock',header:'Status',cell:({getValue})=><Badge className={getValue<boolean>()?'bg-emerald-50 text-emerald-700':'bg-red-50 text-red-700'}>{getValue<boolean>()?'In stock':'Out of stock'}</Badge>},
    {id:'actions',header:'',cell:({row})=><Button variant="outline" size="sm" onClick={()=>onAdjust(row.original)}><Plus size={14}/> Adjust</Button>},
  ],[onAdjust]);
  const table=useReactTable({data:rows,columns,getCoreRowModel:getCoreRowModel()});
  return <Card className="mt-5 overflow-hidden"><div className="overflow-x-auto"><Table className="min-w-[760px]"><TableHeader>{table.getHeaderGroups().map(group=><TableRow key={group.id}>{group.headers.map(cell=><TableHead key={cell.id}>{cell.isPlaceholder?null:flexRender(cell.column.columnDef.header,cell.getContext())}</TableHead>)}</TableRow>)}</TableHeader><TableBody>{table.getRowModel().rows.map(row=><TableRow key={row.id}>{row.getVisibleCells().map(cell=><TableCell key={cell.id}>{flexRender(cell.column.columnDef.cell,cell.getContext())}</TableCell>)}</TableRow>)}</TableBody></Table></div></Card>;
}
