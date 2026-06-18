
export interface ProductResponse {
  id:            number;
  nombre:        string;
  talla:         string;
  color:         string;
  categoria:     string;
  precioCompra:  number;
  precioVenta:   number;
  stock:         number;
  esLiquidacion: boolean;
  estado:        boolean;
}
