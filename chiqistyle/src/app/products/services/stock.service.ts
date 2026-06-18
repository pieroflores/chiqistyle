import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface StockItem {
  idSubProducto: number;
  nombreProducto: string;
  codigoSubProducto: string;
  color: string;
  talla: string;
  precioVenta: number;
  stock: number;
  ubicacion: string;
  fotoProducto: string;
  fechaCompra: string;
  precioVentaLiquidacion: number;
}

@Injectable({ providedIn: 'root' })
export class StockService {
  private apiUrl = 'http://localhost:7038/api/Stock';

  constructor(private http: HttpClient) {}

  listarStock(): Observable<StockItem[]> {
    return this.http.get<StockItem[]>(`${this.apiUrl}/listar`);
  }
}
