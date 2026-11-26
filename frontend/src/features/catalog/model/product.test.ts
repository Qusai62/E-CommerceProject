import {describe,expect,it} from 'vitest';
import {deviceCategory,toProductDetail} from './product';

describe('Product detail adapter',()=>{
  it('recognizes variants without assuming optional fields exist',()=>{
    const result=toProductDetail({productId:42,name:'Apex laptop',variants:[{productVariantId:10,sku:'AST-1',price:49,isActive:true}]});
    expect(result?.id).toBe(42);
    expect(result?.variants[0]).toEqual({id:10,sku:'AST-1',price:49,isActive:true});
  });
  it('handles untyped or malformed product detail bodies',()=>{
    expect(toProductDetail(undefined)).toBeNull();
    expect(toProductDetail({name:'No variants',variants:[{},null]} )?.variants).toEqual([]);
  });
  it('does not map missing variation IDs to a purchasable option',()=>{
    expect(toProductDetail({variants:[{sku:'S-1',price:20}]})?.variants).toHaveLength(0);
  });
  it('classifies devices for image placeholders',()=>{
    expect(deviceCategory('Apex 14 laptop')).toBe('laptop');
    expect(deviceCategory('Wireless Headphones')).toBe('audio');
  });
});
