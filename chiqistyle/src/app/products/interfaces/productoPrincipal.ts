export interface ProductoPrincipal {
  idProductoPrincipal?: number;
  idCategoria: number;
  nombreProducto: string;
  fotoProducto?: string; // aquí se guarda la ruta que devuelve el backend
}
