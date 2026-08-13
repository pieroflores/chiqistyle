import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import type { ProductoPrincipal } from 'src/app/products/interfaces/productoPrincipal';

interface ProductoCatalogo extends ProductoPrincipal {
  id: number;
  precio: number;
  precioOriginal?: number;
  categoria: string;
  descripcion: string;
  esLiquidacion: boolean;
  fotoProducto: string;
  etiqueta?: string;
  departamento: string;
}

@Component({
  selector: 'app-catalogo-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './catalogo-page.component.html',
})
export class CatalogoPageComponent {
  readonly departamentos = [
    { nombre: 'Niña', activo: true },
    { nombre: 'Niño', activo: false, proximamente: true },
    { nombre: 'Mujer', activo: false, proximamente: true }
  ];

  readonly categorias = ['Todos', 'Cartera', 'Conjunto 2 piezas', 'Conjunto 3 piezas', 'Prenda única'];

 readonly productos: ProductoCatalogo[] = [
  {
    id: 1,
    idProductoPrincipal: 101,
    idCategoria: 1,
    nombreProducto: 'Cartera Rosada Mini',
    categoria: 'Cartera',
    descripcion: 'Cartera compacta con acabado premium y estilo diario.',
    precio: 799,
    precioOriginal: 999,
    esLiquidacion: true,
    fotoProducto: 'https://picsum.photos/seed/cartera-rosada-mini/600/800',
    etiqueta: 'Liquidación',
    departamento: 'Niña'
  },
  {
    id: 2,
    idProductoPrincipal: 102,
    idCategoria: 2,
    nombreProducto: 'Conjunto 2 piezas Coral',
    categoria: 'Conjunto 2 piezas',
    descripcion: 'Set fresco y elegante para looks cómodos y modernos.',
    precio: 1299,
    precioOriginal: 1699,
    esLiquidacion: true,
    fotoProducto: 'https://picsum.photos/seed/conjunto-2-coral/600/800',
    etiqueta: 'Oferta',
    departamento: 'Niña'
  },
  {
    id: 3,
    idProductoPrincipal: 103,
    idCategoria: 3,
    nombreProducto: 'Conjunto 3 piezas Bloom',
    categoria: 'Conjunto 3 piezas',
    descripcion: 'Look completo con armonía de tonos y detalles suaves.',
    precio: 1599,
    precioOriginal: 1999,
    esLiquidacion: true,
    fotoProducto: 'https://picsum.photos/seed/conjunto-3-bloom/600/800',
    etiqueta: 'Hot',
    departamento: 'Niña'
  },
  {
    id: 4,
    idProductoPrincipal: 104,
    idCategoria: 4,
    nombreProducto: 'Prenda única Lila',
    categoria: 'Prenda única',
    descripcion: 'Prenda versátil para combinar con accesorios del día a día.',
    precio: 899,
    esLiquidacion: false,
    fotoProducto: 'https://picsum.photos/seed/prenda-unica-lila/600/800',
    departamento: 'Niña'
  },
  {
    id: 5,
    idProductoPrincipal: 105,
    idCategoria: 2,
    nombreProducto: 'Conjunto 2 piezas Bella',
    categoria: 'Conjunto 2 piezas',
    descripcion: 'Diseño femenino con cortes fluidos y gran comodidad.',
    precio: 1399,
    precioOriginal: 1749,
    esLiquidacion: true,
    fotoProducto: 'https://picsum.photos/seed/conjunto-2-bella/600/800',
    etiqueta: 'Sale',
    departamento: 'Niña'
  },
  {
    id: 6,
    idProductoPrincipal: 106,
    idCategoria: 5,
    nombreProducto: 'Prenda única Rosé',
    categoria: 'Prenda única',
    descripcion: 'Estilo limpio y sobrio ideal para vestir con personalidad.',
    precio: 1099,
    esLiquidacion: false,
    fotoProducto: 'https://picsum.photos/seed/prenda-unica-rose/600/800',
    departamento: 'Niña'
  },
  {
    id: 7,
    idProductoPrincipal: 107,
    idCategoria: 3,
    nombreProducto: 'Conjunto 3 piezas Petal',
    categoria: 'Conjunto 3 piezas',
    descripcion: 'Set completo con sensación ligera, fresca y moderna.',
    precio: 1799,
    precioOriginal: 2199,
    esLiquidacion: true,
    fotoProducto: 'https://picsum.photos/seed/conjunto-3-petal/600/800',
    etiqueta: 'Nuevo',
    departamento: 'Niña'
  },
  {
    id: 8,
    idProductoPrincipal: 108,
    idCategoria: 1,
    nombreProducto: 'Cartera Clásica',
    categoria: 'Cartera',
    descripcion: 'Accesorio funcional y chic para complementar cualquier look.',
    precio: 999,
    esLiquidacion: false,
    fotoProducto: 'https://picsum.photos/seed/cartera-clasica/600/800',
    departamento: 'Niña'
  }
];

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
