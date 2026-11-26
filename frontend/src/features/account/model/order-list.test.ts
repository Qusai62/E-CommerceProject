import { describe, expect, it } from 'vitest';
import { parseOrderList } from './order-list';
describe('undocumented order list runtime boundary', () => {
  it('extracts recognized fields from a paginated API response', () => {
    expect(parseOrderList({items:[{orderId:42,orderNumber:'ORD-42',totalAmount:5}],pageNumber:1,totalPages:1,hasNextPage:false},1)?.items[0].id).toBe(42);
  });
  it('rejects malformed order data instead of guessing identifiers', () => {
    expect(parseOrderList({items:[{foo:'bar'}]},1)).toBeUndefined();
  });
});
