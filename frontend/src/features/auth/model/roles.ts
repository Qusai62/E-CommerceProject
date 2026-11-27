/** UI routing only. The API must enforce authorization on every protected endpoint. */
export type AppRole = 'Customer' | 'StoreOwner';

export function hasRole(roles: readonly string[] | null | undefined, role: AppRole): boolean {
  return (roles ?? []).some((value) => value.toLowerCase() === role.toLowerCase());
}

export function preferredDestination(
  roles: readonly string[] | null | undefined,
): '/owner' | '/account' {
  return hasRole(roles, 'StoreOwner') ? '/owner' : '/account';
}
