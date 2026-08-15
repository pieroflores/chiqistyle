import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PRODUCTOS_CATALOGO, type ProductoCatalogo } from '../../data/productos.mock';

@Component({
  selector: 'app-catalogo-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './catalogo-page.component.html',
})
export class CatalogoPageComponent {
  readonly departamentos = [
    { nombre: 'Niña', activo: true },
    { nombre: 'Niño', activo: false, proximamente: true },
    { nombre: 'Mujer', activo: false, proximamente: true }
  ];

  readonly categorias = ['Todos', 'Cartera', 'Conjunto 2 piezas', 'Conjunto 3 piezas', 'Prenda única'];

 readonly productos: ProductoCatalogo[] = PRODUCTOS_CATALOGO;

  getBadgeClasses(etiqueta?: string): string {
    const base = 'inline-flex items-center rounded-full border px-2.5 py-1 text-[10px] font-bold uppercase tracking-[0.18em]';

    switch (etiqueta) {
      case 'Liquidación':
        return `${base} border-pink-200 bg-pink-100 text-pink-700`;
      case 'Oferta':
        return `${base} border-rose-200 bg-rose-100 text-rose-700`;
      case 'Hot':
        return `${base} border-orange-200 bg-orange-100 text-orange-700`;
      case 'Nuevo':
        return `${base} border-violet-200 bg-violet-100 text-violet-700`;
      case 'Sale':
        return `${base} border-amber-200 bg-amber-100 text-amber-700`;
      default:
        return `${base} border-pink-200 bg-pink-100 text-pink-700`;
    }
  }
}
