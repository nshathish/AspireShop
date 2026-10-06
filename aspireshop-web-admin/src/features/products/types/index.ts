export type Product = {
  id: string;
  name: string;
  imageUrl: string | null;
  price: number;
  stock: number;
  categoryId: string | null;
};

export type ProductInput = {
  name: string;
  price: number;
  stock: number;
  categoryId?: string | null;
  imageUrl?: string | null;
};
