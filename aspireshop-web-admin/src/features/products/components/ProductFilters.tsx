import { ChevronDown, Search, X } from 'lucide-react';

import type { Category } from '@/features/categories/types';

interface ProductFiltersProps {
  categories: Category[];
  search: string;
  categoryId: string;
  status: string;
  minPrice: string;
  maxPrice: string;
  onSearchChange: (value: string) => void;
  onCategoryChange: (value: string) => void;
  onStatusChange: (value: string) => void;
  onMinPriceChange: (value: string) => void;
  onMaxPriceChange: (value: string) => void;
  onClear: () => void;
}

export default function ProductFilters({
  categories,
  search,
  categoryId,
  status,
  minPrice,
  maxPrice,
  onSearchChange,
  onCategoryChange,
  onStatusChange,
  onMinPriceChange,
  onMaxPriceChange,
  onClear,
}: ProductFiltersProps) {
  return (
    <div className="space-y-4 border-b border-slate-100 p-4">
      <label className="relative block">
        <span className="sr-only">Search products</span>
        <Search size={17} className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
        <input
          value={search}
          onChange={(event) => onSearchChange(event.target.value)}
          placeholder="Search products..."
          className="h-11 w-full rounded-lg border border-slate-200 bg-white pl-10 pr-4 text-sm outline-none transition placeholder:text-slate-400 focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
        />
      </label>

      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-[1.2fr_1fr_1fr_1fr_auto]">
        <label className="relative">
          <span className="sr-only">Filter by category</span>
          <select value={categoryId} onChange={(event) => onCategoryChange(event.target.value)} className="h-10 w-full appearance-none rounded-lg border border-slate-200 bg-white px-3 pr-9 text-sm text-slate-700 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100">
            <option value="all">All Categories</option>
            {categories.map((category) => (
              <option key={category.id} value={category.id}>{category.name}</option>
            ))}
          </select>
          <ChevronDown className="pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" size={16} />
        </label>

        <label className="relative">
          <span className="sr-only">Filter by status</span>
          <select value={status} onChange={(event) => onStatusChange(event.target.value)} className="h-10 w-full appearance-none rounded-lg border border-slate-200 bg-white px-3 pr-9 text-sm text-slate-700 outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100">
            <option value="all">All Status</option>
            <option value="active">Active</option>
            <option value="out-of-stock">Out of stock</option>
          </select>
          <ChevronDown className="pointer-events-none absolute right-3 top-1/2 -translate-y-1/2 text-slate-400" size={16} />
        </label>

        <input type="number" min="0" value={minPrice} onChange={(event) => onMinPriceChange(event.target.value)} placeholder="Min Price" className="h-10 rounded-lg border border-slate-200 px-3 text-sm outline-none placeholder:text-slate-400 focus:border-blue-500 focus:ring-2 focus:ring-blue-100" />
        <input type="number" min="0" value={maxPrice} onChange={(event) => onMaxPriceChange(event.target.value)} placeholder="Max Price" className="h-10 rounded-lg border border-slate-200 px-3 text-sm outline-none placeholder:text-slate-400 focus:border-blue-500 focus:ring-2 focus:ring-blue-100" />
        <button type="button" onClick={onClear} className="inline-flex h-10 items-center justify-center gap-2 rounded-lg border border-slate-200 px-4 text-sm font-medium text-slate-600 transition hover:bg-slate-50">
          <X size={15} />
          Clear
        </button>
      </div>
    </div>
  );
}
