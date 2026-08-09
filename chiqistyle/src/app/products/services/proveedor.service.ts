import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Cliente } from '@products/interfaces/cliente.interface';
import { Proveedor } from '@products/interfaces/proveedor.interface';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({providedIn: 'root'})
export class ProveedorService {

    private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/proveedor/`; // cambia por tu endpoint real


    AddProveedor(proveedor: Proveedor): Observable<Proveedor> {
     // console.log(proveedor)
       return this.http.post<Proveedor>(this.apiUrl, proveedor);
    }
    getProveedor(): Observable<Proveedor[]>{
      return this.http.get<Proveedor[]>(`${this.apiUrl}`)
      .pipe(tap((resp) => console.log(resp)));
    }
    updateProveedor(id: number, proveedor: Proveedor): Observable<Proveedor> {
    return this.http.put<Proveedor>(`${this.apiUrl}${id}`, proveedor);
    }
    deleteProveedor(id: number) {
  return this.http.delete(`${this.apiUrl}${id}`);
}

}
