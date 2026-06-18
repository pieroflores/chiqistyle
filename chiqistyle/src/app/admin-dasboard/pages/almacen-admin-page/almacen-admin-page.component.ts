// src/app/products/almacen-admin-page/almacen-admin-page.component.ts
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Almacen, AlmacenService, Ubicacion } from '@products/services/almacen.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-almacen-admin-page',
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './almacen-admin-page.component.html',
  // ...
})
export class AlmacenAdminPageComponent implements OnInit {
  formAlmacen!: FormGroup;
  almacenes: Almacen[] = [];
  ubicaciones: Ubicacion[] = [];
  editando: boolean = false;
  almacenSeleccionado?: Almacen;

  constructor(private fb: FormBuilder, private almacenService: AlmacenService) {}

  ngOnInit(): void {
    this.inicializarFormulario();
    this.cargarAlmacenes();
    this.cargarUbicaciones();
  }

 inicializarFormulario(): void {
  this.formAlmacen = this.fb.group({
    idAlmacen: [null],
    idUbicacion: [null, Validators.required],
    seccion: ['', Validators.required],
    // 🚨 CAMBIO CLAVE: Inicializar con null o 0 para tipos number, no con ''
    columna: [null, [Validators.required, Validators.min(1)]],
    nivel: [null, [Validators.required, Validators.min(1)]],
    descripcion: ['', Validators.required]
  });
}

  // ... (cargarAlmacenes y cargarUbicaciones permanecen igual) ...
  cargarAlmacenes(): void {
    this.almacenService.listarAlmacenes().subscribe({
      next: (data) => (this.almacenes = data),
      error: (err) => console.error('Error al listar almacenes', err)
    });
  }

  cargarUbicaciones(): void {
    this.almacenService.listarUbicaciones().subscribe({
      next: (data) => (this.ubicaciones = data),
      error: (err) => console.error('Error al listar ubicaciones', err)
    });
  }

  guardarAlmacen(): void {
    if (this.formAlmacen.invalid) {
     Swal.fire({
        icon: 'error',
        title: 'Formulario Inválido',
        text: 'Por favor, completa todos los campos requeridos y revisa las validaciones.',
      });
      return;
    }

    const nuevoAlmacen: Almacen = this.formAlmacen.value;
    console.log(this.editando)
    if (this.editando) {
      // Cuando editas, el idAlmacen ya está en el formulario y se envía.
      this.almacenService.editarAlmacen(nuevoAlmacen).subscribe({
        next: () => {
          this.cargarAlmacenes();
          this.cancelarEdicion();
          Swal.fire({
            icon: 'success',
            title: '¡Actualizado!',
            text: 'El almacén ha sido modificado con éxito.',
            timer: 2000,
            showConfirmButton: false
          });
        },
       error: (err) => {
          // 🔴 Manejo de error
          console.error('Error al editar almacén', err);
          Swal.fire('Error', 'Hubo un error al actualizar el almacén.', 'error');
        }

      });
    } else {
      // Cuando registras, el idAlmacen se envía como null/0, lo cual es correcto.
      this.almacenService.registrarAlmacen(nuevoAlmacen).subscribe({
        next: () => {
          this.cargarAlmacenes();
          this.formAlmacen.reset();
          Swal.fire({
            icon: 'success',
            title: '¡Registrado!',
            text: 'El nuevo almacén ha sido guardado con éxito.',
            timer: 2000,
            showConfirmButton: false
          });
        },
       error: (err) => {
          // 🔴 Manejo de error
          console.error('Error al registrar almacén', err);
          Swal.fire('Error', 'Hubo un error al registrar el almacén.', 'error');
        }
      });
    }
  }

editar(almacen: Almacen): void {
  this.editando = true;
  this.almacenSeleccionado = almacen;
  // Usar patchValue con los valores numéricos
  this.formAlmacen.patchValue({
    idAlmacen: almacen.idAlmacen,
    idUbicacion: almacen.idUbicacion,
    seccion: almacen.seccion,
    // Aseguramos que se parchean como números
    columna: Number(almacen.columna),
    nivel: Number(almacen.nivel),
    descripcion: almacen.descripcion,
  });
}

  // ... (eliminar y cancelarEdicion permanecen igual) ...
 eliminar(id: number): void {
    // Reemplazar confirm() por Swal.fire
    Swal.fire({
      title: '¿Estás seguro?',
      text: '¡No podrás revertir esta acción!',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#d33',
      cancelButtonColor: '#3085d6',
      confirmButtonText: 'Sí, ¡eliminar!',
      cancelButtonText: 'Cancelar'
    }).then((result) => {
      if (result.isConfirmed) {
        this.almacenService.eliminarAlmacen(id).subscribe({
          next: () => {
            this.cargarAlmacenes();
            // 🟢 Éxito al eliminar
            Swal.fire(
              '¡Eliminado!',
              'El almacén ha sido eliminado.',
              'success'
            );
          },
          error: (err) => {
             // 🔴 Manejo de error
            console.error('Error al eliminar almacén', err);
            Swal.fire('Error', 'Hubo un error al eliminar el almacén.', 'error');
          }
        });
      }
    });
  }

  cancelarEdicion(): void {
    this.editando = false;
    this.almacenSeleccionado = undefined;
    this.formAlmacen.reset();
  }
}
