import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormsModule,ReactiveFormsModule,Validators } from '@angular/forms';
import { Cliente } from '@products/interfaces/cliente.interface';
import { ClienteService } from '@products/services/cliente.service';
import Swal from 'sweetalert2';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-cliente-admin-page',
  imports: [ReactiveFormsModule,CommonModule,FormsModule],
  templateUrl: './cliente-admin-page.component.html',
})
export class ClienteAdminPageComponent implements OnInit{
  busqueda: string = '';   // lo que escribe el usuario
  clientesFiltradas: Cliente[] = []; // copia filtrada
  clientes: Cliente[] = [];
  clienteEnEdicion: number | null = null;

ClienteService= inject(ClienteService);
fb = inject(FormBuilder)
http = inject(HttpClient);
clienteForm = this.fb.group({
    NombreCliente: ['',Validators.required],
    DNI: ['',Validators.required],
     Direccion: [''],   // 👈 ya no obligatorio
  telefono: ['', [
    Validators.pattern(/^\d{9}$/) // 👈 EXACTAMENTE 9 números
  ]]
    // Direccion: ['',Validators.required],
    // telefono: ['',Validators.required],
  })

  ngOnInit(): void {
    this.cargarCliente();
 }
 cargarCliente() {
  this.ClienteService.getCliente().subscribe({
    next: (resp) => {
      this.clientes = resp;
      this.clientesFiltradas = resp; // inicial igual
    },
    error: (err) => {
      console.error('Error al cargar Cliente:', err);
    }
    });
  }
  filtrarCliente() {
  const termino = this.busqueda.toLowerCase().trim();

  this.clientesFiltradas = this.clientes.filter(t => {
    // Convierte todos los valores del objeto talla en un string
    return Object.values(t).some(val =>
      String(val).toLowerCase().includes(termino)
    );
    });
  }

   editarCliente(c: Cliente) {
    // Llenar el formulario con los valores actuales
    this.clienteForm.setValue({
      NombreCliente: c.nombreCliente,
      DNI: c.dni.toString(),
      Direccion: c.direccion,
      telefono: c.telefono.toString()
    });
this.clienteEnEdicion = c.idCliente;

  }
  cancelarEdicion() {
  this.clienteForm.reset();
  this.clienteEnEdicion = null;
  }

  onSubmit() {
    if (!this.clienteForm.valid) {
    Swal.fire({
      icon: 'warning',
      title: 'Faltan datos',
      text: 'Por favor complete todos los campos obligatorios',
      confirmButtonText: 'Aceptar'
    });
    return;
  }

  const dni = this.clienteForm.value.DNI;

  const existe = this.clientes.some(c => c.dni == dni);

   if (existe && !this.clienteEnEdicion) {
    Swal.fire({
      icon: 'warning',
      title: 'Cliente ya registrado',
      text: 'Este DNI ya se encuentra registrado'
    });
    return;
  }

  const telefonoValor = this.clienteForm.value.telefono;
  const telefono = telefonoValor && !isNaN(Number(telefonoValor))
  ? parseInt(telefonoValor)
  : null;
    const cliente: Cliente = {
      nombreCliente: this.clienteForm.value.NombreCliente!,
      dni: this.clienteForm.value.DNI!,
      direccion: this.clienteForm.value.Direccion!,
     // telefono: parseInt(this.clienteForm.value.telefono!)
telefono:  telefono
    };

    if (this.clienteEnEdicion) {
      // 👇 Modo edición
      this.ClienteService.updateCliente(this.clienteEnEdicion, cliente).subscribe({
        next: () => {
          Swal.fire('Actualizado', 'El cliente se actualizó correctamente', 'success');
          this.clienteForm.reset();
          this.clienteEnEdicion = null;
          this.cargarCliente();
        },
        error: () => Swal.fire('Error', 'No se pudo actualizar el cliente', 'error')
      });
      this.busqueda=''
    } else {
      // 👇 Modo registro nuevo
      this.ClienteService.AddCliente(cliente).subscribe({
        next: () => {
          Swal.fire('Registrado', 'El cliente se registró correctamente', 'success');
          this.clienteForm.reset();
          this.cargarCliente();
          window.location.reload();
        },
        error: () => Swal.fire('Error', 'No se pudo registrar el cliente', 'error')
      });
    }
  }

buscarDni() {

  const dni = this.clienteForm.value.DNI;

  if (!dni || dni.length !== 8) return;

  this.ClienteService.buscarDni(dni).subscribe({

    next: (resp) => {

      let nombre = '';

      // 🔹 API 1
      if (resp.nombre) {
        nombre = resp.nombre;
      }

      // 🔹 API 2
      else if (resp.data && resp.data.nombre_completo) {
        nombre = resp.data.nombre_completo;
      }

      if (nombre) {
        this.clienteForm.patchValue({
          NombreCliente: nombre
        });
      } else {
        this.mostrarManual(dni);
      }

    },

    error: () => {
      this.mostrarManual(dni);
    }

  });

}

mostrarManual(dni: string){
  this.clienteForm.patchValue({
    NombreCliente: `CLIENTE ${dni}`
  });

  Swal.fire({
    icon: 'info',
    title: 'DNI no encontrado',
    text: 'Ingrese el nombre manualmente'
  });
}

  eliminarCliente(id: number) {
    Swal.fire({
      title: '¿Estás seguro?',
      text: 'Esta acción no se puede deshacer',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Sí, eliminar',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.ClienteService.deleteCliente(id).subscribe({
          next: () => {
            Swal.fire('Eliminado', 'La cliente fue eliminada correctamente', 'success');
            this.cargarCliente(); // refresca la tabla
            this.busqueda=''
          },
          error: (err) => {
            Swal.fire('Error', 'No se pudo eliminar la cliente', 'error');
          }
        });
      }
    });
  }

 }
