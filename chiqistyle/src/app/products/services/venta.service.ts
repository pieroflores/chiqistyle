import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { VentaPendiente } from '@products/interfaces/venta.interface';

export interface ProductoVentaInfo {
  idAlmacen: number;
  precioVenta: number;
  ubicacionTexto: string;
  cantidadDisponible: number;
}

@Injectable({
  providedIn: 'root'
})
export class VentaService {
  private apiUrl = 'http://localhost:7038/api/Venta';

  constructor(private http: HttpClient) {}

  obtenerInfoProducto(idSubProducto: number): Observable<ProductoVentaInfo[]> {
    return this.http.get<ProductoVentaInfo[]>(`${this.apiUrl}/ConsultarProducto/${idSubProducto}`);
  }
  uploadComprobante(file: File) {
  const formData = new FormData();
  formData.append('file', file);
  return this.http.post<{ path: string }>(`${this.apiUrl}/uploadComprobante`, formData);
}
registrarVenta(data: any) {
  return this.http.post(`${this.apiUrl}`, data);
}
listarPendientes(): Observable<VentaPendiente[]> {
    return this.http.get<VentaPendiente[]>(`${this.apiUrl}/ListarPendientes`);
  }
obtenerDetalleVenta(idVenta: number) {
  return this.http.get<any[]>(`${this.apiUrl}/ObtenerDetalleVenta/${idVenta}`);
}
listarPagosPorVenta(idVenta: number) {
  return this.http.get<any[]>(`${this.apiUrl}/pagos/${idVenta}`);
}

  registrarPagoAdicional(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/RegistrarPagoAdicional`, data);
  }
  getClientesDeuda(){
    return this.http.get<any[]>(`${this.apiUrl}/clientes-deuda`);
  }

  pagarCliente(data:any){
    return this.http.post(`${this.apiUrl}/pagar-cliente`,data);
  }
}
