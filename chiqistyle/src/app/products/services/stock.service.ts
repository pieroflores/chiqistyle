import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
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
  precioCompra: number;
  largoPantalon: number;
  entrepierna: number;
}

@Injectable({ providedIn: 'root' })
export class StockService {
  private apiUrl = `${environment.apiUrl}/Stock`;

  constructor(private http: HttpClient) {}

  listarStock(): Observable<StockItem[]> {
    return this.http.get<StockItem[]>(`${this.apiUrl}/listar`);
  }
}
