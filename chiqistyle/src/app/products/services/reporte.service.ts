import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface DetalleCompletoVenta {
   pagos: any[];     // Antes era Pagos
    productos: any[]; // Antes era Productos
}
@Injectable({
  providedIn: 'root'
})

export class ReporteService {
  private apiUrl = 'http://localhost:7038/api/Reporte';

  constructor(private http: HttpClient) {}

  obtenerVentas(params: any): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Ventas`, { params });
  }

  obtenerCompras(params: any): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Compras`, { params });
  }

  obtenerPagosPorCliente(params: any): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/PagosPorCliente`, { params });
  }
  obtenerDetalleCompletoVenta(idVenta: number): Observable<DetalleCompletoVenta> {
    return this.http.get<DetalleCompletoVenta>(`${this.apiUrl}/ObtenerDetalleCompletoVenta/${idVenta}`);
  }
  actualizarEnvio(idVenta: number, enviado: boolean) {
  return this.http.put(`${this.apiUrl}/ActualizarEnvio/${idVenta}`, {
    enviado: enviado
  });
}
}
