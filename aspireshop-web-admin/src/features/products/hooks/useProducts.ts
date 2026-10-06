import { useQuery } from '@tanstack/react-query';

import { getProducts } from '@/features/products/api/productsApi.ts';

export function useProducts() {
  return useQuery({
    queryKey: ['products'],
    queryFn: ({signal}) => getProducts(signal),
  });
}
