import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import type { ProductoCatalogo } from '../interfaces/catalogo.interface';

@Injectable({ providedIn: 'root' })
export class CatalogoService {
  private readonly apiUrl = `${environment.apiUrl}/store`;

  constructor(private readonly http: HttpClient) {}

  obtenerCatalogo(): Observable<ProductoCatalogo[]> {
    return this.http.get<ProductoCatalogo[]>(`${this.apiUrl}/catalogo`);
  }
}
