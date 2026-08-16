export interface VarianteCatalogo {
  idSubProducto: number;
  codigoSubProducto: string;
  color: string;
  talla: string;
  stock: number;
  precioVenta: number;
  precioVentaLiquidacion: number;
  foto: string;
}

export interface ProductoCatalogo {
  idProductoPrincipal: number;
  nombreProducto: string;
  categoria: string;
  fotoProducto: string;
  precioVenta: number;
  precioVentaLiquidacion: number;
  variantes: VarianteCatalogo[];
}
