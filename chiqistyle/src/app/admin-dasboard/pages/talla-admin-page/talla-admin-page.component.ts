import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Talla } from '@products/interfaces/talla.interface';
import Swal from 'sweetalert2';
import { CommonModule } from '@angular/common';
import { TallaService } from '@products/services/talla.service';

@Component({
  selector: 'app-talla-admin-page',
  imports: [ReactiveFormsModule,CommonModule,FormsModule],
  templateUrl: './talla-admin-page.component.html',
})

export class TallaAdminPageComponent implements OnInit{
  busqueda: string = '';   // lo que escribe el usuario
  tallasFiltradas: Talla[] = []; // copia filtrada
  tallas: Talla[] = [];
  tallaEnEdicion: number | null = null;

TallaService = inject(TallaService);
  fb = inject(FormBuilder)
  tallaForm = this.fb.group({
    NombreTalla: ['',Validators.required],
    Abreviatura: ['',Validators.required]
  })


 ngOnInit(): void {
this.cargarTallas();
 this.detectarCambioTalla();
 }
 cancelarEdicion() {
  this.tallaForm.reset();
  this.tallaEnEdicion = null;
}
detectarCambioTalla(): void {
  this.tallaForm.get('NombreTalla')?.valueChanges.subscribe(valor => {
    if (!valor) return;

    const abrev = this.generarAbreviaturaTalla(valor.toString());
    this.tallaForm.get('Abreviatura')?.setValue(abrev, { emitEvent: false });
  });
}
generarAbreviaturaTalla(valor: string): string {
  const limpio = valor.trim().toUpperCase();

  // Si es numérico y corto → copiar igual
  if (/^\d+$/.test(limpio) && limpio.length <= 2) {
    return limpio;
  }

  const palabras = limpio.split(' ').filter(p => p.length > 0);

  // Si tiene 2 o más palabras → ABC-DEF
  if (palabras.length > 1) {
    const p1 = palabras[0].substring(0, 3);
    const p2 = palabras[1].substring(0, 3);
    return `${p1}-${p2}`;
  }

  // Texto largo simple → primeras 3 letras
  return limpio.substring(0, 3);
}
 onSubmit() {
  if (!this.tallaForm.valid) return;

  const talla: Talla = {
    nombreTalla: this.tallaForm.value.NombreTalla!,
    abreviatura: this.tallaForm.value.Abreviatura!
  };

  if (this.tallaEnEdicion) {
    // 👇 Modo edición
    this.TallaService.updateTalla(this.tallaEnEdicion, talla).subscribe({
      next: () => {
        Swal.fire('Actualizado', 'La talla se actualizó correctamente', 'success');
        this.tallaForm.reset();
        this.tallaEnEdicion = null;
        this.cargarTallas();
      },
      error: () => Swal.fire('Error', 'No se pudo actualizar la talla', 'error')
    });
    this.busqueda=''
  } else {
    // 👇 Modo registro nuevo
    this.TallaService.AddTalla(talla).subscribe({
      next: () => {
        Swal.fire('Registrado', 'La talla se registró correctamente', 'success');
        this.tallaForm.reset();
        this.cargarTallas();
      },
      error: () => Swal.fire('Error', 'No se pudo registrar la talla', 'error')
    });
  }
}

eliminarTalla(id: number) {
  Swal.fire({
    title: '¿Estás seguro?',
    text: 'Esta acción no se puede deshacer',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Sí, eliminar',
    cancelButtonText: 'Cancelar'
  }).then((result) => {
    if (result.isConfirmed) {
      this.TallaService.deleteTalla(id).subscribe({
        next: () => {
          Swal.fire('Eliminado', 'La talla fue eliminada correctamente', 'success');
          this.cargarTallas(); // refresca la tabla
          this.busqueda=''
        },
        error: (err) => {
          Swal.fire('Error', 'No se pudo eliminar la talla', 'error');
        }
      });
    }
  });
}



  cargarTallas() {
  this.TallaService.getTalla().subscribe({
    next: (resp) => {
      this.tallas = resp;
      this.tallasFiltradas = resp; // inicial igual
    },
    error: (err) => {
      console.error('Error al cargar tallas:', err);
    }
  });
}
filtrarTallas() {
  const termino = this.busqueda.toLowerCase().trim();

  this.tallasFiltradas = this.tallas.filter(t => {
    // Convierte todos los valores del objeto talla en un string
    return Object.values(t).some(val =>
      String(val).toLowerCase().includes(termino)
    );
  });
}

  editarTalla(t: Talla) {
  // Llenar el formulario con los valores actuales
  this.tallaForm.setValue({
    NombreTalla: t.nombreTalla,
    Abreviatura: t.abreviatura
  });

  // Guardar el ID de la talla que estamos editando
  this.tallaEnEdicion = t.idTalla;

}

 }

