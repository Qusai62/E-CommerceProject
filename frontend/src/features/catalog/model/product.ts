import type { ProductListItemResponse } from '@/shared/api/generated/api.generated';

export type ProductVariantVM = { id: number; sku: string; price: number; isActive: boolean };
export type ProductImageVM = { id?: number; storageKey: string; altText: string; sortOrder: number; isPrimary: boolean };
export type ProductSpecVM = { id?: number; name: string; value: string; sortOrder: number };
export type ProductDetailVM = {
  id: number; name: string; description: string; price: number | null;
  imageKey: string | null; images: ProductImageVM[];
  variants: ProductVariantVM[]; specs: ProductSpecVM[];
};
const record = (value: unknown): Record<string, unknown> | null => value !== null && typeof value === 'object' && !Array.isArray(value) ? value as Record<string, unknown> : null;
const str = (v: unknown) => typeof v === 'string' ? v : '';
const num = (v: unknown) => typeof v === 'number' && Number.isFinite(v) ? v : undefined;

// GET /api/products/{id} is untyped in Swagger. Validate individual fields and degrade gracefully.
// Replace this adapter with generated DTOs once the backend publishes the response schema.
export function toProductDetail(raw: unknown, fallback?: ProductListItemResponse): ProductDetailVM | null {
  const item = record(raw);
  if (!item && !fallback) return null;
  const images: ProductImageVM[] = Array.isArray(item?.images) ? item.images.map(record).filter((i): i is Record<string, unknown> => !!i).map(i => ({
    id: num(i.productImageId), storageKey: str(i.storageKey), altText: str(i.altText),
    sortOrder: num(i.sortOrder) ?? 0, isPrimary: i.isPrimary === true,
  })).sort((a, b) => a.sortOrder - b.sortOrder) : [];
  const variants: ProductVariantVM[] = Array.isArray(item?.variants) ? item.variants.map(record).filter((v): v is Record<string, unknown> => !!v).flatMap(v => {
    const id = num(v.productVariantId); if (id === undefined) return [];
    return [{ id, sku: str(v.sku), price: num(v.price) ?? 0, isActive: v.isActive !== false }];
  }) : [];
  const specs: ProductSpecVM[] = Array.isArray(item?.technicalSpecifications) ? item.technicalSpecifications.map(record).filter((v): v is Record<string, unknown> => !!v).map(v => ({
    id: num(v.technicalSpecificationId), name: str(v.name), value: str(v.value), sortOrder: num(v.sortOrder) ?? 0,
  })).sort((a, b) => a.sortOrder - b.sortOrder) : [];
  return {
    id: num(item?.productId) ?? fallback?.productId ?? 0,
    name: str(item?.name) || fallback?.name || 'Product',
    description: str(item?.description),
    price: num(item?.priceFrom) ?? fallback?.priceFrom ?? (variants.length ? Math.min(...variants.map(v => v.price)) : null),
    imageKey: images.find(i => i.isPrimary)?.storageKey || images[0]?.storageKey || fallback?.primaryImageStorageKey || null,
    images, variants, specs,
  };
}
export type DeviceCategory='laptop'|'phone'|'audio'|'display'|'watch'|'camera'|'gaming'|'keyboard'|'mouse'|'speaker'|'other';
export function deviceCategory(name:string):DeviceCategory{
  const value=name.toLowerCase();
  if(value.includes('laptop')||value.includes('notebook')||value.includes('ultrabook'))return 'laptop';
  if(value.includes('phone'))return 'phone';
  if(value.includes('headphone')||value.includes('earbud'))return 'audio';
  if(value.includes('monitor')||value.includes('display'))return 'display';
  if(value.includes('watch'))return 'watch';
  if(value.includes('camera'))return 'camera';
  if(value.includes('gaming')||value.includes('console'))return 'gaming';
  if(value.includes('keyboard'))return 'keyboard';
  if(value.includes('mouse'))return 'mouse';
  if(value.includes('speaker'))return 'speaker';
  return 'other';
}
