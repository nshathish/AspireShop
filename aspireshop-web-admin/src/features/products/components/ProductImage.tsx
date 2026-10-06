import { useState } from 'react';
import { ImageOff } from 'lucide-react';

import type { Product } from '@/features/products/types';

export default function ProductImage({ product }: { product: Product }) {
  const [hasError, setHasError] = useState(false);

  if (!product.imageUrl || hasError) {
    return (
      <span className="grid size-11 place-items-center rounded-lg bg-slate-100 text-slate-400">
        <ImageOff size={17} />
      </span>
    );
  }

  return (
    <img
      src={product.imageUrl}
      alt=""
      className="size-11 rounded-lg bg-slate-100 object-contain"
      onError={() => setHasError(true)}
    />
  );
}
