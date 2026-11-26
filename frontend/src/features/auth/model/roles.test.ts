import { describe, expect, it } from 'vitest';
import { hasRole, preferredDestination } from './roles';
describe('server-provided roles', () => {
  it('identifies StoreOwner without assuming the first role', () => {
    expect(hasRole(['Customer', 'StoreOwner'], 'StoreOwner')).toBe(true);
    expect(preferredDestination(['Customer', 'StoreOwner'])).toBe('/owner');
  });
  it('does not grant owner access to guests or customers', () => {
    expect(hasRole(['Customer'], 'StoreOwner')).toBe(false);
    expect(hasRole(undefined, 'StoreOwner')).toBe(false);
    expect(preferredDestination(['Customer'])).toBe('/account');
  });
});
