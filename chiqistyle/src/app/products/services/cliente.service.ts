import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Cliente } from '@products/interfaces/cliente.interface';
import { Observable, tap } from 'rxjs';

@Injectable({providedIn: 'root'})
export class ClienteService {

    private http = inject(HttpClient);
  private apiUrl = 'http://localhost:7038/api/cliente/'; // cambia por tu endpoint real


    AddCliente(cliente: Cliente): Observable<Cliente> {
       return this.http.post<Cliente>(this.apiUrl, cliente);
    }
    getCliente(): Observable<Cliente[]>{
      return this.http.get<Cliente[]>(`${this.apiUrl}`)
      .pipe(tap((resp) => console.log(resp)));
    }
    updateCliente(id: number, cliente: Cliente): Observable<Cliente> {
    return this.http.put<Cliente>(`${this.apiUrl}${id}`, cliente);
    }
    deleteCliente(id: number) {
  return this.http.delete(`${this.apiUrl}${id}`);
}
buscarDni(dni: string): Observable<any> {
  return this.http.get<any>(`${this.apiUrl}dni/${dni}`);
}


}
