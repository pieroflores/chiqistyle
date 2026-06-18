import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Categoria {
  // Asegúrate de que el nombre de la propiedad coincida con el backend (MCategoria)
  idCategoria?: number; // Nullable/opcional para la inserción
  nombreCategoria: string;
}

@Injectable({
  providedIn: 'root'
})
export class CategoriaService {
  private apiUrl = 'http://localhost:7038/api'; // 🚨 VERIFICA TU PUERTO BASE

  constructor(private http: HttpClient) {}

  // 🔹 GET: Listar todos
  listarCategorias(): Observable<Categoria[]> {
    return this.http.get<Categoria[]>(`${this.apiUrl}/Categoria`);
  }

  // 🔹 POST: Insertar
  registrarCategoria(categoria: Categoria): Observable<any> {
    console.log(categoria.nombreCategoria)
    return this.http.post<any>(`${this.apiUrl}/Categoria`, categoria);
  }

  // 🔹 PUT: Editar (El backend espera el ID en la URL y el cuerpo)
  editarCategoria(categoria: Categoria): Observable<any> {
    // Usamos el idCategoria para la URL
    return this.http.put<any>(`${this.apiUrl}/Categoria/${categoria.idCategoria}`, categoria);
  }

  // 🔹 DELETE: Eliminar
  eliminarCategoria(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/Categoria/${id}`);
  }
}
