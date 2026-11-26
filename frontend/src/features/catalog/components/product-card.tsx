import { Link } from '@tanstack/react-router';
import { ArrowUpRight, Camera, Cpu, Gamepad2, Headphones, Keyboard, Laptop, Monitor, Mouse, Smartphone, Speaker, Watch } from 'lucide-react';
import type { ProductListItemResponse } from '@/shared/api/generated/api.generated';
import { publicImageUrl, currency } from '@/shared/lib/format';
import { Badge } from '@/components/ui/badge';
import {deviceCategory} from '../model/product';

const palette = [
  'from-slate-100 to-indigo-100', 'from-blue-50 to-slate-200',
  'from-cyan-50 to-blue-100', 'from-indigo-50 to-sky-100',
  'from-slate-100 to-violet-100', 'from-sky-50 to-slate-200',
] as const;
function HardwareIcon({ name }:{name:string}){
  const icons={laptop:Laptop,phone:Smartphone,audio:Headphones,display:Monitor,watch:Watch,camera:Camera,gaming:Gamepad2,keyboard:Keyboard,mouse:Mouse,speaker:Speaker,other:Cpu} as const;
  const Icon=icons[deviceCategory(name)];
  return <Icon aria-hidden="true" className="h-[42%] w-[42%] stroke-[1.1] text-slate-600 drop-shadow-2xl transition-transform duration-500 group-hover:scale-110"/>;
}
export function ProductVisual({ name, imageKey, variant = 'card' }:{name:string;imageKey?:string|null;variant?:'card'|'detail'}){
  const url=publicImageUrl(imageKey);
  const background=palette[name.length % palette.length];
  return <div className={`group relative grid place-items-center overflow-hidden bg-gradient-to-br ${background} ${variant==='detail'?'aspect-square rounded-3xl':'aspect-[1.1/1] rounded-2xl'}`}>
    {url?<img src={url} alt={name} loading="lazy" className="absolute inset-0 size-full object-contain p-6"/>:<><span aria-hidden="true" className="absolute inset-6 rounded-full border border-white/70 shadow-[inset_0_0_90px_#ffffffaa]"/><HardwareIcon name={name}/><span className="absolute bottom-4 left-5 rounded-md bg-white/70 px-2 py-1 text-[10px] font-semibold uppercase tracking-wider text-slate-500">Product image unavailable</span></>}
  </div>;
}
export function ProductCard({product}:{product:ProductListItemResponse}){
  const name=product.name||'Unnamed product';
  return <article className="group min-w-0"><Link to="/shop/$productId" params={{ productId:String(product.productId??0) }} className="block rounded-2xl focus-visible:outline-2 focus-visible:outline-offset-4" aria-label={`View ${name}`}>
    <div className="relative"><ProductVisual name={name} imageKey={product.primaryImageStorageKey}/><Badge className="absolute left-4 top-4 border-0 bg-white/95 text-slate-700">Explore</Badge><span className="absolute bottom-4 right-4 grid size-10 place-items-center rounded-xl bg-white shadow-sm group-hover:bg-moss group-hover:text-white"><ArrowUpRight size={17}/></span></div>
    <div className="mt-4 flex items-start justify-between gap-4"><div><h3 className="text-[15px] font-bold leading-6 text-ink group-hover:text-moss">{name}</h3><p className="mt-1 text-xs text-slate-500">Electronics · See details</p></div><span className="shrink-0 text-sm font-bold">{currency(product.priceFrom)}</span></div>
  </Link></article>;
}
