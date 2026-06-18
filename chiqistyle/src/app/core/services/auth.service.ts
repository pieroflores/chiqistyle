// src/app/core/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
 export interface UsuarioResponse {
  idUsuario: number;
  usuarioLogin: string;
  nombreCompleto: string;
  idRol: number;
  nombreRol: string;
  modulo: { nombreModulo: string }[];
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'http://localhost:7038/api/Login';

  constructor(private http: HttpClient) {}

  login(usuario: string, clave: string): Observable<UsuarioResponse> {
    return this.http.post<UsuarioResponse>(this.apiUrl, { usuario, clave }).pipe(
      tap(resp => {
        localStorage.setItem('usuario', JSON.stringify(resp));
      })
    );
  }

  getUsuario(): UsuarioResponse | null {
    const data = localStorage.getItem('usuario');
    return data ? JSON.parse(data) : null;
  }

  logout(): void {
    localStorage.removeItem('usuario');
  }

  estaAutenticado(): boolean {
    return !!localStorage.getItem('usuario');
  }

  tieneAcceso(modulo: string): boolean {
    const usuario = this.getUsuario();
    if (!usuario) return false;
    return usuario.modulo.some(m => m.nombreModulo.toLowerCase() === modulo.toLowerCase());
  }

  obtenerModulos(): string[] {
    const usuario = this.getUsuario();
    return usuario ? usuario.modulo.map(m => m.nombreModulo) : [];
  }
}
