/** Keep the request token in module memory, never in Redux or web storage. */
let csrfRequestToken: string | undefined;
let pendingTokenRequest: Promise<void> | undefined;

type CsrfResponse = { requestToken?: unknown; RequestToken?: unknown };

export function readCsrfRequestToken(): string | undefined {
  return csrfRequestToken;
}

export function setCsrfRequestToken(token: string): void {
  if (!token.trim()) {
    throw new Error('The CSRF endpoint returned an empty request token.');
  }

  csrfRequestToken = token;
}

export function clearCsrfRequestToken(): void {
  csrfRequestToken = undefined;
}

/** Gets an ASP.NET request token and lets the browser retain its paired HttpOnly cookie. */
export async function ensureCsrfRequestToken(): Promise<void> {
  if (csrfRequestToken) return;

  if (!pendingTokenRequest) {
    pendingTokenRequest = (async () => {
      const endpoint = import.meta.env.VITE_CSRF_ENDPOINT ?? '/api/security/csrf';
      const response = await fetch(endpoint, {
        method: 'GET',
        credentials: 'include',
        cache: 'no-store',
        headers: { Accept: 'application/json' },
      });
      if (!response.ok) throw new Error(`Unable to obtain a CSRF token (${response.status}).`);

      let payload: CsrfResponse;
      try {
        payload = (await response.json()) as CsrfResponse;
      } catch {
        throw new Error('The CSRF endpoint returned invalid JSON.');
      }

      const token = payload.requestToken ?? payload.RequestToken;
      if (typeof token !== 'string') {
        throw new Error('The CSRF endpoint did not return requestToken.');
      }
      setCsrfRequestToken(token);
    })().finally(() => {
      pendingTokenRequest = undefined;
    });
  }

  return pendingTokenRequest;
}
