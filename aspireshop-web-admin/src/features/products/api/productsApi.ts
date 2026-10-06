import { catalogHttpClient } from '@/shared/api/catalogHttpClient.ts';
import type { Product, ProductInput } from '@/features/products/types';

const productsPath = '/api/products';

export async function getProducts(signal?: AbortSignal): Promise<Product[]> {
  const response = await catalogHttpClient.get<Product[]>(productsPath, {
    signal,
  });
  return response.data;
}

export async function getProduct(
  id: string,
  signal?: AbortSignal,
): Promise<Product> {
  const response = await catalogHttpClient.get<Product>(
    `${productsPath}/${id}`,
    { signal },
  );
  return response.data;
}

export async function createProduct(product: ProductInput): Promise<Product> {
  const response = await catalogHttpClient.post<Product>(
    `${productsPath}/`,
    product,
  );
  return response.data;
}

export async function updateProduct(
  id: string,
  product: ProductInput,
): Promise<void> {
  await catalogHttpClient.put(`${productsPath}/${id}`, product);
}

export async function deleteProduct(id: string): Promise<void> {
  await catalogHttpClient.delete(`${productsPath}/${id}`);
}
