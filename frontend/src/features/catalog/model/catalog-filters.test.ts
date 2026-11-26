import { describe, expect, it } from 'vitest';
import { parseOptionalNumber, toProductQuery } from './catalog-filters';
describe('catalog filter mapping',()=>{
  it('keeps URL inputs separate from the API DTO',()=>{
    expect(toProductQuery({q:'headphones',page:2,sort:'price-asc',min:20})).toEqual({
      PageNumber:2,PageSize:9,Search:'headphones',SortBy:'price',SortDirection:'asc',
      MinPrice:20,MaxPrice:undefined,BrandId:undefined,CategoryId:undefined,
    });
  });
  it('does not convert empty values to zero',()=>{
    expect(parseOptionalNumber('')).toBeUndefined();
    expect(parseOptionalNumber('-9')).toBeUndefined();
    expect(parseOptionalNumber('25.50')).toBe(25.5);
  });
});
