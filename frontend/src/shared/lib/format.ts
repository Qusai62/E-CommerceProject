export const currency = (value: number | null | undefined) =>
  value === null || value === undefined
    ? 'Price on request'
    : new Intl.NumberFormat('en-JO', {
        style: 'currency',
        currency: import.meta.env.VITE_CURRENCY || 'JOD',
      }).format(value);
export const date = (value?: string) =>
  value ? new Intl.DateTimeFormat('en-JO', { dateStyle: 'medium' }).format(new Date(value)) : '—';
export function publicImageUrl(storageKey?: string | null): string | undefined {
  const cdn = import.meta.env.VITE_IMAGE_BASE_URL;
  // Never interpret untrusted storage keys as arbitrary image URLs.
  if (!cdn || !storageKey || storageKey.includes('..') || storageKey.startsWith('/')) {
    return undefined;
  }
  return `${cdn.replace(/\/$/, '')}/${storageKey.split('/').map(encodeURIComponent).join('/')}`;
}
