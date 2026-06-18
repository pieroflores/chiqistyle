// Define las interfaces que coinciden con el objeto MDatosComboCompra de C#
export interface SubProductoCombo {
  idSubProducto: number;
  productoVariable: string;
}

export interface AlmacenCombo {
  idAlmacen: number;
  ubicacion: string;
}
export interface ProductoDisponibles {
  idSubProducto: number;
  productoVariable: string;
  fotoProducto?: string;
}
export interface productoPrincipal{
  IdProductoPrincipal: number;
  NombreProducto: string;
}

export interface TipoDocumento {
  idTipoDocumentoMercaderia: number;
  nombre: string; // Asegúrate que este nombre coincida con tu modelo C#
}

export interface DatosComboCompra {
  productos: SubProductoCombo[];
  ubicaciones: AlmacenCombo[];
  tiposDocumento: TipoDocumento[];
  productoDisponibles:ProductoDisponibles[]
  productoPrincipal: productoPrincipal[]
}

export interface CompraEnviar {
  idProveedor: number;
  fechaCompra: Date;
  idUsuario: number;
  documentoReferencia: string;
  observacion?: string;
  detalle: CompraProductoEnviar[]; // <- antes tenías CompraProductoE[]
}

export interface CompraProductoEnviar {
  // idProveedor: number;
  // fechaCompra: Date;
  //  idSubProducto: number;
  // cantidad: number;
  // precioUnitario: number;
  // idAlmacen: number;
  // idUsuario?: string;
  // tipoDocumento: string;
  // observacion?: string;
  idSubProducto: number;
  cantidad: number;
  precioUnitario: number;
  idAlmacen: number;

}

export interface CompraProductoList {
  Proveedor: string;
  fechaCompra: Date;
  TipoDocumento: string;
  observacion?: string;
  SubProducto: string;
  cantidad: number;
  precioUnitario: number;
  Almacen: string;
}
