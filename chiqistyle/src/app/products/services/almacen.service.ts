import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Ubicacion {
  idUbicacion: number;
  nombreUbicacion: string;
}

export interface Almacen {
  idAlmacen?: number;
  idUbicacion: number;
  seccion: string;
  columna: number;
  nivel: number;
  descripcion: string;
  nombreUbicacion?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AlmacenService {
  private apiUrl = 'http://localhost:7038/api';
  constructor(private http: HttpClient) {}

  // 🔹 Listar almacenes
  listarAlmacenes(): Observable<Almacen[]> {
    return this.http.get<Almacen[]>(`${this.apiUrl}/Almacen/Listar`);
  }

  // 🔹 Registrar
  registrarAlmacen(almacen: Almacen): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/Almacen/Registrar`, almacen);
  }

  // 🔹 Editar
  editarAlmacen(almacen: Almacen): Observable<void> {
    console.log("service")
    return this.http.put<void>(`${this.apiUrl}/Almacen/Editar`, almacen);
  }

  // 🔹 Eliminar
  eliminarAlmacen(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/Almacen/Eliminar/${id}`);
  }

  // 🔹 Listar ubicaciones (para combo)
  listarUbicaciones(): Observable<Ubicacion[]> {
    return this.http.get<Ubicacion[]>(`${this.apiUrl}/Ubicacion/Listar`);
  }
}
