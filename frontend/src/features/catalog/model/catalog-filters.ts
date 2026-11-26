export type CatalogSearch={q:string;page:number;sort:string;min?:number;max?:number;brand?:number;category?:number};
export type CatalogFilterChange=Partial<CatalogSearch>;
export function toProductQuery(search:CatalogSearch){
  return {
    PageNumber:search.page,PageSize:9,Search:search.q||undefined,
    SortBy:search.sort.startsWith('price')?'price':search.sort.startsWith('name')?'name':'createdAt',
    SortDirection:search.sort.endsWith('desc')?'desc':'asc',
    MinPrice:search.min,MaxPrice:search.max,BrandId:search.brand,CategoryId:search.category,
  };
}
export function parseOptionalNumber(value:string){
  if(!value.trim())return undefined;
  const number=Number(value);
  return Number.isFinite(number)&&number>=0?number:undefined;
}
