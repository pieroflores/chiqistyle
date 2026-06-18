import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import Swal from 'sweetalert2';
import { Categoria, CategoriaService } from '@products/services/categoria.service';

@Component({
  selector: 'app-categoria-admin-page',
  standalone: true, // Asumiendo que usas componentes standalone
  imports: [ReactiveFormsModule, CommonModule, FormsModule],
  templateUrl: './categoria-admin-page.component.html' // Puedes omitir si no tienes estilos
})
export class CategoriaAdminPageComponent implements OnInit {

  formCategoria!: FormGroup;
  categorias: Categoria[] = [];
  editando: boolean = false;

  constructor(private fb: FormBuilder, private categoriaService: CategoriaService) {}

  ngOnInit(): void {
    this.inicializarFormulario();
    this.cargarCategorias();
  }

  inicializarFormulario(): void {
    this.formCategoria = this.fb.group({
      // Incluimos idCategoria para la edición
      idCategoria: [null],
      nombreCategoria: ['', Validators.required],
    });
  }

  cargarCategorias(): void {
    this.categoriaService.listarCategorias().subscribe({
      next: (data) => (this.categorias = data),
      error: (err) => {
        console.error('Error al listar categorías', err);
        Swal.fire('Error de Carga', 'No se pudieron cargar las categorías.', 'error');
      }
    });
  }

  // 🔹 CRUD: Guardar (Insertar/Editar)
  guardarCategoria(): void {
    if (this.formCategoria.invalid) {
      Swal.fire('Error', 'Por favor, complete el campo requerido.', 'error');
      return;
    }

    const categoriaPayload: Categoria = this.formCategoria.value;

    Swal.fire({
      title: this.editando ? 'Actualizando...' : 'Registrando...',
      didOpen: () => { Swal.showLoading(); },
      allowOutsideClick: false
    });

    if (this.editando) {
      this.categoriaService.editarCategoria(categoriaPayload).subscribe({
        next: () => {
          Swal.fire('¡Actualizado!', 'La categoría ha sido modificada con éxito.', 'success');
          this.cargarCategorias();
          this.cancelarEdicion();
        },
        error: (err) => {
          console.error('Error al editar categoría', err);
          Swal.fire('Error', 'Hubo un error al actualizar la categoría.', 'error');
        }
      });
    } else {
      this.categoriaService.registrarCategoria(categoriaPayload).subscribe({
        next: () => {
          Swal.fire('¡Registrada!', 'La nueva categoría ha sido guardada con éxito.', 'success');
          this.cargarCategorias();
          this.formCategoria.reset();
        },
        error: (err) => {
          console.error('Error al registrar categoría', err);
          Swal.fire('Error', 'Hubo un error al registrar la categoría.', 'error');
        }
      });
    }
  }

  // 🔹 CRUD: Editar (Cargar datos en el formulario)
  editar(categoria: Categoria): void {
    this.editando = true;
    // Usamos patchValue para rellenar el formulario
    this.formCategoria.patchValue(categoria);
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
        this.categoriaService.eliminarCategoria(id).subscribe({
          next: () => {
            this.cargarCategorias();
            Swal.fire('¡Eliminada!', 'La categoría ha sido eliminada.', 'success');
          },
          error: (err) => {
            console.error('Error al eliminar categoría', err);
            Swal.fire('Error', 'Hubo un error al eliminar la categoría.', 'error');
          }
        });
      }
    });
  }

  // 🔹 Utilidad: Cancelar Edición
  cancelarEdicion(): void {
    this.editando = false;
    this.formCategoria.reset();
    // Asegura que idCategoria se restablezca
    this.formCategoria.get('idCategoria')?.setValue(null);
  }
}
