import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router';
import { Loader2, PackagePlus, Plus } from 'lucide-react';

import Pagination from '@/shared/components/Pagination';
import ProductFilters from '@/features/products/components/ProductFilters.tsx';
import ProductListItem from '@/features/products/components/ProductListItem.tsx';

import { useCategories } from '@/features/categories/hooks/useCategories';
import { useProducts } from '@/features/products/hooks/useProducts';

const pageSize = 5;

export default function ProductListPage() {
  const navigate = useNavigate();

  const {
    data: products = [],
    isLoading: isProductsLoading,
    error: productsError,
    refetch: refetchProducts,
  } = useProducts();
  const {
    data: categories = [],
    isLoading: isCategoriesLoading,
    error: categoriesError,
    refetch: refetchCategories,
  } = useCategories();

  const [search, setSearch] = useState('');
  const [categoryId, setCategoryId] = useState('all');
  const [status, setStatus] = useState('all');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [page, setPage] = useState(1);

  const isLoading = isProductsLoading || isCategoriesLoading;

  const error =
    productsError || categoriesError
      ? 'Unable to load the product catalog. Please try again.'
      : null;

  const categoryNames = useMemo(
    () => new Map(categories.map((category) => [category.id, category.name])),
    [categories],
  );

  const filteredProducts = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase();
    const minimum = minPrice === '' ? null : Number(minPrice);
    const maximum = maxPrice === '' ? null : Number(maxPrice);

    return products.filter((product) => {
      const matchesSearch =
        normalizedSearch === '' ||
        product.name.toLowerCase().includes(normalizedSearch);
      const matchesCategory =
        categoryId === 'all' || product.categoryId === categoryId;
      const matchesStatus =
        status === 'all' ||
        (status === 'active' ? product.stock > 0 : product.stock === 0);
      const matchesMinimum = minimum === null || product.price >= minimum;
      const matchesMaximum = maximum === null || product.price <= maximum;

      return (
        matchesSearch &&
        matchesCategory &&
        matchesStatus &&
        matchesMinimum &&
        matchesMaximum
      );
    });
  }, [categoryId, maxPrice, minPrice, products, search, status]);

  const pageCount = Math.max(1, Math.ceil(filteredProducts.length / pageSize));
  const visibleProducts = filteredProducts.slice(
    (page - 1) * pageSize,
    page * pageSize,
  );
  const firstVisibleProduct =
    filteredProducts.length === 0 ? 0 : (page - 1) * pageSize + 1;
  const lastVisibleProduct = Math.min(page * pageSize, filteredProducts.length);

  const clearFilters = () => {
    setSearch('');
    setCategoryId('all');
    setStatus('all');
    setMinPrice('');
    setMaxPrice('');
  };

  return (
    <section className="mx-auto w-full max-w-[1500px]">
      <div className="mb-6 flex flex-col justify-between gap-4 sm:flex-row sm:items-start">
        <div>
          <h1 className="text-2xl font-bold tracking-tight text-slate-900">
            Products
          </h1>
          <p className="mt-1 text-sm text-slate-500">
            Manage your product catalog
          </p>
        </div>
        <button
          type="button"
          onClick={() => navigate('/products/new')}
          className="inline-flex items-center justify-center gap-2 rounded-lg bg-blue-600 px-4 py-2.5 text-sm font-semibold text-white shadow-sm transition hover:bg-blue-700"
        >
          <Plus size={17} />
          Add Product
        </button>
      </div>

      <div className="rounded-xl border border-slate-200 bg-white shadow-sm">
        <ProductFilters
          categories={categories}
          search={search}
          categoryId={categoryId}
          status={status}
          minPrice={minPrice}
          maxPrice={maxPrice}
          onSearchChange={setSearch}
          onCategoryChange={setCategoryId}
          onStatusChange={setStatus}
          onMinPriceChange={setMinPrice}
          onMaxPriceChange={setMaxPrice}
          onClear={clearFilters}
        />

        {error && (
          <div className="m-4 flex items-center justify-between gap-4 rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
            <span>{error}</span>
            <button
              type="button"
              onClick={() => {
                void Promise.all([refetchProducts(), refetchCategories()]);
              }}
              className="font-semibold underline"
            >
              Retry
            </button>
          </div>
        )}

        <div className="overflow-x-auto">
          <table className="w-full min-w-[850px] text-left text-sm">
            <thead className="bg-slate-50 text-xs font-semibold uppercase tracking-wide text-slate-500">
              <tr>
                <th className="w-12 px-5 py-3">
                  <input
                    type="checkbox"
                    aria-label="Select all products"
                    className="size-4 rounded border-slate-300 text-blue-600"
                  />
                </th>
                <th className="px-3 py-3">Image</th>
                <th className="px-3 py-3">Name</th>
                <th className="px-3 py-3">Category</th>
                <th className="px-3 py-3">Price</th>
                <th className="px-3 py-3">Stock</th>
                <th className="px-3 py-3">Status</th>
                <th className="px-5 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {isLoading && (
                <tr>
                  <td colSpan={8} className="py-16 text-center text-slate-500">
                    <Loader2 className="mx-auto mb-2 animate-spin" size={22} />
                    Loading products...
                  </td>
                </tr>
              )}

              {!isLoading &&
                visibleProducts.map((product) => (
                  <ProductListItem
                    key={product.id}
                    product={product}
                    categoryName={
                      product.categoryId
                        ? (categoryNames.get(product.categoryId) ?? 'Unknown')
                        : 'Uncategorized'
                    }
                    onEdit={(selectedProduct) =>
                      navigate(`/products/${selectedProduct.id}/edit`)
                    }
                    onDuplicate={(selectedProduct) =>
                      navigate(`/products/new?copy=${selectedProduct.id}`)
                    }
                  />
                ))}

              {!isLoading && visibleProducts.length === 0 && (
                <tr>
                  <td colSpan={8} className="py-16 text-center text-slate-500">
                    <PackagePlus
                      className="mx-auto mb-2 text-slate-400"
                      size={26}
                    />
                    No products match the selected filters.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>

        <Pagination
          currentPage={page}
          pageCount={pageCount}
          firstItem={firstVisibleProduct}
          lastItem={lastVisibleProduct}
          totalItems={filteredProducts.length}
          itemLabel="products"
          onPageChange={setPage}
        />
      </div>
    </section>
  );
}
