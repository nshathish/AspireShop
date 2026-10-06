import { useQuery } from '@tanstack/react-query';

import { getCategories } from '@/features/categories/api/categoriesApi.ts';

export function useCategories() {
  return useQuery({
    queryKey: ['categories'],
    queryFn: ({signal}) => getCategories(signal),
  });
}