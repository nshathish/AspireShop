import { Check, Copy, Edit3 } from 'lucide-react';

import ProductImage from '@/features/products/components/ProductImage.tsx';
import type { Product } from '@/features/products/types';

interface ProductListItemProps {
  product: Product;
  categoryName: string;
  onEdit: (product: Product) => void;
  onDuplicate: (product: Product) => void;
}

function formatPrice(price: number) {
  return new Intl.NumberFormat('en-GB', {
    style: 'currency',
    currency: 'GBP',
  }).format(price);
}

export default function ProductListItem({
  product,
  categoryName,
  onEdit,
  onDuplicate,
}: ProductListItemProps) {
  const isActive = product.stock > 0;

  return (
    <tr className="transition hover:bg-slate-50/70">
      <td className="px-5 py-3.5">
        <input type="checkbox" aria-label={`Select ${product.name}`} className="size-4 rounded border-slate-300 text-blue-600" />
      </td>
      <td className="px-3 py-3.5"><ProductImage product={product} /></td>
      <td className="px-3 py-3.5 font-semibold text-slate-800">{product.name}</td>
      <td className="px-3 py-3.5 text-slate-500">{categoryName}</td>
      <td className="px-3 py-3.5 font-medium text-slate-700">{formatPrice(product.price)}</td>
      <td className="px-3 py-3.5 text-slate-600">{product.stock}</td>
      <td className="px-3 py-3.5">
        <span className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-semibold ${isActive ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-100 text-slate-600'}`}>
          <Check size={13} />
          {isActive ? 'Active' : 'Out of stock'}
        </span>
      </td>
      <td className="px-5 py-3.5">
        <div className="flex justify-end gap-1">
          <button type="button" aria-label={`Edit ${product.name}`} onClick={() => onEdit(product)} className="rounded-md p-2 text-slate-500 hover:bg-blue-50 hover:text-blue-600"><Edit3 size={16} /></button>
          <button type="button" aria-label={`Duplicate ${product.name}`} onClick={() => onDuplicate(product)} className="rounded-md p-2 text-slate-500 hover:bg-slate-100 hover:text-slate-800"><Copy size={16} /></button>
        </div>
      </td>
    </tr>
  );
}
