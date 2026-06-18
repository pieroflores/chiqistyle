import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class DashboardService {

    private http = inject(HttpClient);
  private apiUrl = 'http://localhost:7038/api/dashboard'; // cambia por tu endpoint real


  obtenerResumen() {
    return this.http.get<any>(`${this.apiUrl}/resumen`);
  }
}
