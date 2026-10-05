export type Product = {
  id: string
  name: string
  price: number
  stock: number
}

const baseUrl = import.meta.env.VITE_CATALOG_API_URL

export async function getProducts(): Promise<Product[]> {
  const response = await fetch(`${baseUrl}/api/products`)

  if (!response.ok) {
    throw new Error(`Failed to load products: ${response.status}`)
  }

  return response.json()
}