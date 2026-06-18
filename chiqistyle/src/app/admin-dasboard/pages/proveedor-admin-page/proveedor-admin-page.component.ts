import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Proveedor } from '@products/interfaces/proveedor.interface';
import { ProveedorService } from '@products/services/proveedor.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-proveedor-admin-page',
  imports: [ReactiveFormsModule,CommonModule,FormsModule],
  templateUrl: './proveedor-admin-page.component.html',
})
export class ProveedorAdminPageComponent implements OnInit{
  busqueda: string = '';   // lo que escribe el usuario
  proveedorFiltradas: Proveedor[] = []; // copia filtrada
  proveedor: Proveedor[] = [];
  proveedorEnEdicion: number | null = null;
  ProveedorService= inject(ProveedorService);
  fb = inject(FormBuilder)
  proveedorForm = this.fb.group({
     nombreProveedor: ['', Validators.required],
  ruc: ['', Validators.required],
  direccion: [''],
  telefono: [''],
  //email: ['']
  email: ['']
  })
ngOnInit(): void {
    this.cargarProveedor();
 }
 cargarProveedor() {
  this.ProveedorService.getProveedor().subscribe({
    next: (resp) => {
      this.proveedor = resp;
      this.proveedorFiltradas = resp; // inicial igual
    },
    error: (err) => {
      console.error('Error al cargar Proveedor:', err);
    }
    });
  }
  filtrarProveedor() {
  const termino = this.busqueda.toLowerCase().trim();

  this.proveedorFiltradas = this.proveedor.filter(t => {
    // Convierte todos los valores del objeto talla en un string
    return Object.values(t).some(val =>
      String(val).toLowerCase().includes(termino)
    );
    });
  }

  editarProveedor(p: Proveedor) {
      // Llenar el formulario con los valores actuales
      this.proveedorForm.setValue({
        nombreProveedor: p.nombreProveedor,
        ruc: p.ruc,
        direccion: p.direccion,
        telefono: p.telefono,
        email: p.email
      });
      this.proveedorEnEdicion = p.idProveedor;

    }
    cancelarEdicion() {
    this.proveedorForm.reset();
    this.proveedorEnEdicion = null;
    }

    onSubmit() {
        if (this.proveedorForm.invalid) {
    this.proveedorForm.markAllAsTouched();

    Swal.fire({
      icon: 'warning',
      title: 'Faltan datos',
      text: 'Por favor complete todos los campos obligatorios',
      confirmButtonText: 'Aceptar'
    });

    return;
  }


        const proveedor: Proveedor = {
   nombreProveedor: this.proveedorForm.value.nombreProveedor!,
  ruc: this.proveedorForm.value.ruc!,
  direccion: this.proveedorForm.value.direccion || '',
  telefono: this.proveedorForm.value.telefono || '',
  email: this.proveedorForm.value.email || ''
        };

        if (this.proveedorEnEdicion) {
          // 👇 Modo edición
            console.log("Hola")
          this.ProveedorService.updateProveedor(this.proveedorEnEdicion, proveedor).subscribe({
            next: () => {
              Swal.fire('Actualizado', 'El Proveedor se actualizó correctamente', 'success');
              this.proveedorForm.reset();
              this.proveedorEnEdicion = null;
              this.cargarProveedor();
            },
            error: () => Swal.fire('Error', 'No se pudo actualizar el Proveedor', 'error')
          });
          this.busqueda=''
        } else {
          // 👇 Modo registro nuevo

          this.ProveedorService.AddProveedor(proveedor).subscribe({
            next: () => {
              Swal.fire('Registrado', 'El proveedor se registró correctamente', 'success');
              this.proveedorForm.reset();
              this.cargarProveedor();
            },
            error: () => Swal.fire('Error', 'No se pudo registrar el proveedor', 'error')
          });
        }
      }

      eliminarProveedor(id: number) {
          Swal.fire({
            title: '¿Estás seguro?',
            text: 'Esta acción no se puede deshacer',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Sí, eliminar',
            cancelButtonText: 'Cancelar'
          }).then((result) => {
            if (result.isConfirmed) {
              this.ProveedorService.deleteProveedor(id).subscribe({
                next: () => {
                  Swal.fire('Eliminado', 'El proveedor fue eliminado correctamente', 'success');
                  this.cargarProveedor(); // refresca la tabla
                  this.busqueda=''
                },
                error: (err) => {
                  Swal.fire('Error', 'No se pudo eliminar el proveedor', 'error');
                }
              });
            }
          });
        }

}
