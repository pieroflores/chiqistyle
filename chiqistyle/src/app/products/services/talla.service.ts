import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Talla } from '@products/interfaces/talla.interface';
import { Observable, tap } from 'rxjs';

@Injectable({providedIn: 'root'})
export class TallaService {

    private http = inject(HttpClient);
  private apiUrl = 'http://localhost:7038/api/talla/'; // cambia por tu endpoint real


    AddTalla(talla: Talla): Observable<Talla> {
       return this.http.post<Talla>(this.apiUrl, talla);
    }
    getTalla(): Observable<Talla[]>{
      return this.http.get<Talla[]>(`${this.apiUrl}`)
      .pipe(tap((resp) => console.log(resp)));
    }
    updateTalla(id: number, talla: Talla): Observable<Talla> {
    return this.http.put<Talla>(`${this.apiUrl}${id}`, talla);
    }
    deleteTalla(id: number) {
  return this.http.delete(`${this.apiUrl}${id}`);
}

}
