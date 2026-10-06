import { RouterProvider } from 'react-router';

import { router } from '@/app/router/routes.tsx';
import { QueryProvider } from '@/app/providers/QueryProvider.tsx';

export function AppRouter() {
  return (
    <QueryProvider>
      <RouterProvider router={router} />
    </QueryProvider>
  );
}
