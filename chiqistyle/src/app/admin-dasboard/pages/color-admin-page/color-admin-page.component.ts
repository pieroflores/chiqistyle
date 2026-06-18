// src/app/color-admin-page/color-admin-page.component.ts
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2'; // Importar Swal
import { Color,ColorService } from '@products/services/color.service';
// 🚨 Asegúrate de que la ruta a tu servicio de Color sea correcta

@Component({
  selector: 'app-color-admin-page',
  // Asegúrate de importar los módulos necesarios
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './color-admin-page.component.html'
})
export class ColorAdminPageComponent implements OnInit {

  formColor!: FormGroup;
  colores: Color[] = [];
  editando: boolean = false;
  colorSeleccionado?: Color;

  constructor(private fb: FormBuilder, private ColorService: ColorService) {}

  ngOnInit(): void {
    this.inicializarFormulario();
    this.cargarColores();
     this.detectarCambioNombre();
  }

  inicializarFormulario(): void {
    this.formColor = this.fb.group({
      // Incluimos idColor para la edición
      idColor: [null],
      nombreColor: ['', Validators.required],
      abreviatura: ['', Validators.required],
    });
  }
detectarCambioNombre(): void {
  this.formColor.get('nombreColor')?.valueChanges.subscribe(valor => {
    if (!valor) return;

    const abrev = this.generarAbreviatura(valor);

    this.formColor.get('abreviatura')?.setValue(abrev, { emitEvent: false });
  });
}
generarAbreviatura(texto: string): string {
  const palabras = texto.trim().toUpperCase().split(' ').filter(p => p.length > 0);

  if (palabras.length === 1) {
    return palabras[0].substring(0, 3);
  }

  const p1 = palabras[0].substring(0, 3);
  const p2 = palabras[1].substring(0, 4);

  return `${p1}-${p2}`;
}
  cargarColores(): void {
    this.ColorService.listarColores().subscribe({
      next: (data) => (this.colores = data),
      error: (err) => {
        console.error('Error al listar colores', err);
        Swal.fire('Error de Carga', 'No se pudieron cargar los colores.', 'error');
      }
    });
  }

  // 🔹 CRUD: Guardar (Insertar/Editar)
  guardarColor(): void {
    if (this.formColor.invalid) {
      Swal.fire('Error', 'Por favor, complete todos los campos requeridos.', 'error');
      return;
    }

    const nuevoColor: Color = this.formColor.value;

    Swal.fire({
      title: this.editando ? 'Actualizando...' : 'Registrando...',
      didOpen: () => { Swal.showLoading(); },
      allowOutsideClick: false
    });

    if (this.editando) {
      this.ColorService.editarColor(nuevoColor).subscribe({
        next: () => {
          Swal.fire('¡Actualizado!', 'El color ha sido modificado con éxito.', 'success');
          this.cargarColores();
          this.cancelarEdicion();
        },
        error: (err) => {
          console.error('Error al editar color', err);
          Swal.fire('Error', 'Hubo un error al actualizar el color.', 'error');
        }
      });
    } else {
      this.ColorService.registrarColor(nuevoColor).subscribe({
        next: () => {
          Swal.fire('¡Registrado!', 'El nuevo color ha sido guardado con éxito.', 'success');
          this.cargarColores();
          this.formColor.reset();
        },
        error: (err) => {
          console.error('Error al registrar color', err);
          Swal.fire('Error', 'Hubo un error al registrar el color.', 'error');
        }
      });
    }
  }

  // 🔹 CRUD: Editar (Cargar datos en el formulario)
  editar(color: Color): void {
    this.editando = true;
    this.colorSeleccionado = color;
    // Usamos patchValue para rellenar el formulario
    this.formColor.patchValue(color);
  }

  // 🔹 CRUD: Eliminar
  eliminar(id: number): void {
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
        this.ColorService.eliminarColor(id).subscribe({
          next: () => {
            this.cargarColores();
            Swal.fire('¡Eliminado!', 'El color ha sido eliminado.', 'success');
          },
          error: (err) => {
            console.error('Error al eliminar color', err);
            Swal.fire('Error', 'Hubo un error al eliminar el color.', 'error');
          }
        });
      }
    });
  }

  // 🔹 Utilidad: Cancelar Edición
  cancelarEdicion(): void {
    this.editando = false;
    this.colorSeleccionado = undefined;
    this.formColor.reset();
    // Asegura que idColor se restablezca
    this.formColor.get('idColor')?.setValue(null);
  }
}
