import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { CompraEnviar, CompraProductoEnviar, DatosComboCompra } from '@products/interfaces/compra.interface';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({providedIn: 'root'})
export class CompraService {

    private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Compra`; // cambia por tu endpoint real



    cargarCombos(): Observable<DatosComboCompra> {
    return this.http.get<DatosComboCompra>(`${this.apiUrl}/Combos`);
    }
    AddCompra(compra: CompraEnviar): Observable<CompraEnviar> {
           return this.http.post<CompraEnviar>(this.apiUrl, compra);
    }
    obtenerSubProductosPorProducto(idProducto: number): Observable<any[]> {
  return this.http.get<any[]>(`${this.apiUrl}/ObtenerSubProductos/${idProducto}`);
}
cargarProductosPorProveedor(idProveedor: number) {
  return this.http.get<any[]>(
    `${this.apiUrl}/ProductosPorProveedor/${idProveedor}`
  );
}


}
