import { createBrowserRouter, Navigate } from 'react-router';

import ShopLayout from '@/app/layouts/ShopLayout.tsx';
import ProductListPage from '@/features/products/pages/ProductListPage.tsx';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <ShopLayout />,
    children: [
      {
        index: true,
        element: <Navigate to="/dashboard" replace />,
      },
      // dashboard route
      {
        path: 'dashboard',
        element: <div>Dashboard</div>,
      },
      // products route
      {
        path: 'products',
        children: [
          {
            index: true,
            element: <ProductListPage />,
          },
          {
            path: 'new',
            element: <div>New Product</div>,
          },
          {
            path: ':productId',
            element: <div>Product Details</div>,
          },
        ],
      },
      // categories route
      {
        path: 'categories',
        element: <div>Categories</div>,
      },
      // orders route
      {
        path: 'orders',
        element: <div>Orders</div>,
      },
    ],
  },
]);
