import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ProductResponse } from '@products/interfaces/product.interface';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({providedIn: 'root'})
export class ProductsService {

    private http = inject(HttpClient);
    getProducts(): Observable<ProductResponse[]> {
      return this.http.get<ProductResponse[]>(`${environment.apiUrl}/productos`)
      .pipe(tap((resp) => console.log(resp)));
    }




}
