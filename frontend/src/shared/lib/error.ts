import type { FetchBaseQueryError } from '@reduxjs/toolkit/query';
export function getErrorMessage(error: unknown): string {
  if (!error) return 'Something went wrong.';
  if (typeof error === 'object' && 'status' in error) {
    const e = error as FetchBaseQueryError;
    if (e.status === 'FETCH_ERROR') {
      return 'Unable to connect to the API. Check that your ASP.NET server is running and the proxy is configured.';
    }
    if (e.status === 401) return 'Please sign in to continue.';
    if (e.status === 403) return 'Your account does not have permission to perform this action.';
    if (e.status === 503) {
      return extractDetail(e.data) || 'This service is temporarily unavailable. Please try again.';
    }
    if (e.status === 422) {
      return (
        extractDetail(e.data) ||
        'Some checkout details need your attention. Please review your cart and delivery address.'
      );
    }
    if (e.status === 409) {
      return extractDetail(e.data) || 'This action conflicts with the current state.';
    }
    return extractDetail(e.data) || `Request failed (${String(e.status)}).`;
  }
  return error instanceof Error ? error.message : 'Something went wrong.';
}
function extractDetail(data: unknown): string | undefined {
  if (Array.isArray(data)) {
    const details = data.filter(
      (item): item is { message: string } =>
        item !== null &&
        typeof item === 'object' &&
        'message' in item &&
        typeof item.message === 'string',
    );
    if (details.length) return details.map((item) => item.message).join(' · ');
  }
  if (!data || typeof data !== 'object') return;
  const result = data as { detail?: unknown; title?: unknown; errors?: Record<string, string[]> };
  if (typeof result.detail === 'string' && result.detail) return result.detail;
  if (result.errors) return Object.values(result.errors).flat().join(' ');
  if (typeof result.title === 'string') return result.title;
}
