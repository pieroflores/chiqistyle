import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { inject, Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class DashboardService {

    private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/dashboard`; // cambia por tu endpoint real


  obtenerResumen() {
    return this.http.get<any>(`${this.apiUrl}/resumen`);
  }
}
