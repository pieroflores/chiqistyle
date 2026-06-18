// src/app/services/color.service.ts (o donde tengas tus servicios)
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Color {
  idColor?: number; // Nullable/opcional para la inserción
  nombreColor: string;
  abreviatura: string;
}

@Injectable({
  providedIn: 'root'
})
export class ColorService {
  private apiUrl = 'http://localhost:7038/api'; // 🚨 VERIFICA TU PUERTO BASE

  constructor(private http: HttpClient) {}

  // 🔹 GET: Listar todos
  listarColores(): Observable<Color[]> {
    return this.http.get<Color[]>(`${this.apiUrl}/color`);
  }

  // 🔹 POST: Insertar
  registrarColor(color: Color): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/color`, color);
  }

  // 🔹 PUT: Editar (El backend espera el ID en la URL y el cuerpo)
  editarColor(color: Color): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/color/${color.idColor}`, color);
  }

  // 🔹 DELETE: Eliminar
  eliminarColor(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/color/${id}`);
  }
}
