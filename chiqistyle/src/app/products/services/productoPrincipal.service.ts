import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Categoria } from '@products/interfaces/categoria.interface';
import { Cliente } from '@products/interfaces/cliente.interface';
import { Color } from '@products/interfaces/color.interface';
import { ProductoPrincipal } from '@products/interfaces/productoPrincipal';
import { Talla } from '@products/interfaces/talla.interface';
import { Observable, tap } from 'rxjs';
import { SubProducto } from '../interfaces/subProducto.interface';

@Injectable({ providedIn: 'root' })
export class productoService {
  private apiUrl = 'http://localhost:7038/api/ProductoPrincipal/';

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
    return this.http.post("http://localhost:7038/api/subproductos",subProducto)
  }
     getCategoria(): Observable<Categoria[]>{
          return this.http.get<Categoria[]>("http://localhost:7038/api/categoria")
          .pipe(tap((resp) => console.log(resp)));
        }

         getProducto(): Observable<ProductoPrincipal[]>{
          return this.http.get<ProductoPrincipal[]>(`${this.apiUrl}`)
          .pipe(tap((resp) => console.log(resp)));
        }

        getColor(): Observable<Color[]>{
          return this.http.get<Color[]>("http://localhost:7038/api/color")
          .pipe(tap((resp) => console.log(resp)));
        }

         getTalla(): Observable<Talla[]>{
          return this.http.get<Talla[]>("http://localhost:7038/api/talla")
          .pipe(tap((resp) => console.log(resp)));
        }
        getSubProductosPorProducto(idProductoPrincipal: number): Observable<SubProducto[]> {
  return this.http.get<SubProducto[]>(`http://localhost:7038/api/subproductos/producto/${idProductoPrincipal}`)
    .pipe(tap((resp) => console.log("📥 Subproductos:", resp)));
}

}
