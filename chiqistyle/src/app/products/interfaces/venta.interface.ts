export interface listprodEnviar {
 idCliente: number;
 fechaVenta: Date;
 idSubProducto: number;
 cantidad: number;
 precioUnitario: number;
 idAlmacen: number;
 idUsuario: number;
 montoPagado: number;
 metodoPago : string;
 comprobante?: string;
}
export interface listprodMostrar {
 cliente: string;
 fechaVenta: Date;
 SubProducto: string;
 cantidad: number;
 precioUnitario: number;
 almacen: string;
 usuario: string;
 //usaPrecioPorMayor?: boolean; // 👈 NUEVO: Guardado para el mapeo final
 idSubProducto?: number;
 idAlmacen?: number;
 usaPrecioPorMayor?: number;
}
export interface ProductoVentaInfo {
  idAlmacen: number;
  precioVenta: number; // Precio Normal
  precioVentaPorMayor: number; // 👈 NUEVO: Agregado en el SP y modelo de C#
  precioVentaLiquidacion: number;
  ubicacionTexto: string;
  cantidadDisponible: number;
}

export interface VentaProductoEnviar {
  idSubProducto: number;
  cantidad: number;
  precioUnitario: number;
  idAlmacen: number;
 usaPrecioPorMayor: number;

}

export interface VentaEnviar {
  idCliente: number;
 fechaVenta: Date;
 idUsuario: number;
  detalle: VentaProductoEnviar[]; // <- antes tenías CompraProductoE[]
  montoPagado: number;
 metodoPago : string;
 comprobante?: string;
 tipoTransaccion: string;
}
// export interface VentaPendiente {
//   idVenta: number;
//   nombreCliente: string;
//   dni: string;
//   fechaVenta: string;
//   total: number;
//   montoPagado: number;
//   montoPendiente: number;
//   estado: string;
//   metodoPago: string;
//   comprobante: string;
//   tipoTransaccion: string;
// }

export interface VentaPendiente {
  idVenta: number;
  idCliente: number;
  nombreCliente: string;
  dni?: string;
  fechaVenta: string;

  total: number;
  montoPagado: number;
  montoPendiente: number;

  estado: string;

  metodoPago?: string;
  comprobante?: string;

  tipoTransaccion?: string;
}
export interface ClienteDeuda {
  idCliente: number;
  nombreCliente: string;
  dni: string;

  totalVentas: number;
  totalPagado: number;
  totalPendiente: number;
}
