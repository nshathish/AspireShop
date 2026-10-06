import { createHttpClient } from '@/shared/api/httpClient.ts';

export const catalogHttpClient = createHttpClient(
  import.meta.env.VITE_CATALOG_API_URL,
);
