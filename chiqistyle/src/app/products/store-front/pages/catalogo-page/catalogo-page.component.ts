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
}

@Component({
  selector: 'app-catalogo-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './catalogo-page.component.html',
})
export class CatalogoPageComponent {
  readonly productos: ProductoCatalogo[] = [
    {
      id: 1,
      idProductoPrincipal: 101,
      idCategoria: 1,
      nombreProducto: 'Vestido Rosa Elegance',
      categoria: 'Vestidos',
      descripcion: 'Diseño femenino con corte fluido y detalles suaves.',
      precio: 1299,
      precioOriginal: 1699,
      esLiquidacion: true,
      fotoProducto: 'https://placehold.co/400x400/ff7eb6/ffffff?text=Vestido+Rosa',
      etiqueta: 'Liquidación'
    },
    {
      id: 2,
      idProductoPrincipal: 102,
      idCategoria: 2,
      nombreProducto: 'Blusa Satinada',
      categoria: 'Blusas',
      descripcion: 'Blusa moderna en tejido satinado con acabados premium.',
      precio: 899,
      precioOriginal: 1199,
      esLiquidacion: true,
      fotoProducto: 'https://placehold.co/400x400/f9a8d4/ffffff?text=Blusa',
      etiqueta: 'Oferta'
    },
    {
      id: 3,
      idProductoPrincipal: 103,
      idCategoria: 3,
      nombreProducto: 'Bolso de mano Chic',
      categoria: 'Accesorios',
      descripcion: 'Accesorio versátil para looks casuales y formales.',
      precio: 749,
      precioOriginal: 999,
      esLiquidacion: true,
      fotoProducto: 'https://placehold.co/400x400/f472b6/ffffff?text=Bolso',
      etiqueta: 'Hot'
    },
    {
      id: 4,
      idProductoPrincipal: 104,
      idCategoria: 4,
      nombreProducto: 'Tacones Rosa',
      categoria: 'Calzado',
      descripcion: 'Tacones con confort y estilo para cualquier ocasión.',
      precio: 1599,
      esLiquidacion: false,
      fotoProducto: 'https://placehold.co/400x400/fdba74/ffffff?text=Tacones',
    },
    {
      id: 5,
      idProductoPrincipal: 105,
      idCategoria: 5,
      nombreProducto: 'Set de Pijama',
      categoria: 'Lencería',
      descripcion: 'Conjunto suave y cómodo para días relajados.',
      precio: 699,
      precioOriginal: 899,
      esLiquidacion: true,
      fotoProducto: 'https://placehold.co/400x400/f9a8d4/ffffff?text=Pijama',
      etiqueta: 'Sale'
    },
    {
      id: 6,
      idProductoPrincipal: 106,
      idCategoria: 6,
      nombreProducto: 'Chaqueta de Punto',
      categoria: 'Abrigos',
      descripcion: 'Chaqueta ligera perfecta para clima templado.',
      precio: 1399,
      esLiquidacion: false,
      fotoProducto: 'https://placehold.co/400x400/fbcfe8/ffffff?text=Chaqueta',
    },
    {
      id: 7,
      idProductoPrincipal: 107,
      idCategoria: 7,
      nombreProducto: 'Sandalias Urbanas',
      categoria: 'Calzado',
      descripcion: 'Estilo casual y cómodo para uso diario.',
      precio: 799,
      precioOriginal: 1099,
      esLiquidacion: true,
      fotoProducto: 'https://placehold.co/400x400/f9a8d4/ffffff?text=Sandalias',
      etiqueta: 'Nuevo'
    },
    {
      id: 8,
      idProductoPrincipal: 108,
      idCategoria: 8,
      nombreProducto: 'Reloj Minimal',
      categoria: 'Accesorios',
      descripcion: 'Diseño minimalista para un look elegante y moderno.',
      precio: 1099,
      esLiquidacion: false,
      fotoProducto: 'https://placehold.co/400x400/ddd6fe/ffffff?text=Reloj',
    }
  ];
}
