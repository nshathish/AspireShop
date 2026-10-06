import { catalogHttpClient } from '@/shared/api/catalogHttpClient.ts';
import type { Category } from '@/features/categories/types';

const categoriesPath = '/api/categories';

export async function getCategories(signal?: AbortSignal): Promise<Category[]> {
  const response = await catalogHttpClient.get<Category[]>(categoriesPath, { signal });
  return response.data;
}
