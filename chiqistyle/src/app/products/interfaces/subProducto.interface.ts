export interface SubProducto {
  idSubProducto?: number;
  idProductoPrincipal: number;
  idColor: number;
  idTalla: number;
  precioCompra: number;  // 👈 number, no string
  precioVenta: number;   // 👈 number, no string
   precioVentaPorMayor: number;
   precioVentaLiquidacion?: number;
  codigoSubProducto?: string;
   nombreColor?: string;
  nombreTalla?: string;
}
