import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Categoria } from '@products/interfaces/categoria.interface';
import { Cliente } from '@products/interfaces/cliente.interface';
import { Color } from '@products/interfaces/color.interface';
import { ProductoPrincipal } from '@products/interfaces/productoPrincipal';
import { Talla } from '@products/interfaces/talla.interface';
import { Observable, tap } from 'rxjs';
import { SubProducto } from '../interfaces/subProducto.interface';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class productoService {
  private apiUrl = `${environment.apiUrl}/ProductoPrincipal/`;
  private apiSubUrl = `${environment.apiUrl}/subproductos`;
  constructor(private http: HttpClient) {}

  uploadImage(file: File): Observable<{ path: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<{ path: string }>(`${this.apiUrl}upload`, formData);
  }

  addProducto(producto: any): Observable<any> {
    return this.http.post(this.apiUrl, producto);
  }
  addSubProducto(subProducto:any): Observable<any>{
    return this.http.post(`${this.apiSubUrl}`,subProducto)
  }
  actualizarSubProducto(sub:any){
     return this.http.put(`${this.apiSubUrl}/ActualizarSubProducto`,sub);
  }
  eliminarSubProducto(id:number){
    return this.http.delete(`${this.apiSubUrl}/EliminarSubProducto/${id}`);
  }
     getCategoria(): Observable<Categoria[]>{
         return this.http.get<Categoria[]>(`${environment.apiUrl}/categoria`)
          .pipe(tap((resp) => console.log(resp)));
        }

         getProducto(): Observable<ProductoPrincipal[]>{
          return this.http.get<ProductoPrincipal[]>(`${this.apiUrl}`)
          .pipe(tap((resp) => console.log(resp)));
        }
        getProductoVariante():Observable<ProductoPrincipal[]>{
          return this.http.get<ProductoPrincipal[]>(`${this.apiUrl}variante`)
          .pipe(tap((resp) => console.log(resp)));
        }

         getColor(): Observable<Color[]>{
          return this.http.get<Color[]>(`${environment.apiUrl}/color`)
          .pipe(tap((resp) => console.log(resp)));
        }

         getTalla(): Observable<Talla[]>{
          return this.http.get<Talla[]>(`${environment.apiUrl}/talla`)
          .pipe(tap((resp) => console.log(resp)));
        }
        getSubProductosPorProducto(idProductoPrincipal: number): Observable<SubProducto[]> {
  return this.http.get<SubProducto[]>(`${this.apiSubUrl}/producto/${idProductoPrincipal}`)
    .pipe(tap((resp) => console.log("📥 Subproductos:", resp)));
}

}
