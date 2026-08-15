import type { ProductoPrincipal } from 'src/app/products/interfaces/productoPrincipal';

export interface ProductoCatalogo extends ProductoPrincipal {
  id: number;
  precio: number;
  precioOriginal?: number;
  categoria: string;
  descripcion: string;
  esLiquidacion: boolean;
  fotoProducto: string;
  etiqueta?: string;
  departamento: string;
  tallas: string[];
  colores: Array<{ nombre: string; hex: string }>;
}

export const PRODUCTOS_CATALOGO: ProductoCatalogo[] = [
  {
    id: 1,
    idProductoPrincipal: 101,
    idCategoria: 1,
    nombreProducto: 'Cartera Rosada Mini',
    categoria: 'Cartera',
    descripcion: 'Cartera compacta con acabado premium y estilo diario, ideal para acompañarte durante todo el día con un look fresco y femenino.',
    precio: 799,
    precioOriginal: 999,
    esLiquidacion: true,
    fotoProducto: 'https://picsum.photos/seed/cartera-rosada-mini/600/800',
    etiqueta: 'Liquidación',
    departamento: 'Niña',
    tallas: ['S', 'M', 'L'],
    colores: [
      { nombre: 'Rosado', hex: '#f472b6' },
      { nombre: 'Blanco', hex: '#fdf2f8' },
      { nombre: 'Plomo', hex: '#e2e8f0' }
    ]
  },
  {
    id: 2,
    idProductoPrincipal: 102,
    idCategoria: 2,
    nombreProducto: 'Conjunto 2 piezas Coral',
    categoria: 'Conjunto 2 piezas',
    descripcion: 'Set fresco y elegante para looks cómodos y modernos, pensado para días activos con detalle sofisticado y gran libertad de movimiento.',
    precio: 1299,
    precioOriginal: 1699,
    esLiquidacion: true,
    fotoProducto: 'https://picsum.photos/seed/conjunto-2-coral/600/800',
    etiqueta: 'Oferta',
    departamento: 'Niña',
    tallas: ['XS', 'S', 'M', 'L'],
    colores: [
      { nombre: 'Coral', hex: '#fb7185' },
      { nombre: 'Crema', hex: '#fef3c7' },
      { nombre: 'Rosa', hex: '#f9a8d4' }
    ]
  },
  {
    id: 3,
    idProductoPrincipal: 103,
    idCategoria: 3,
    nombreProducto: 'Conjunto 3 piezas Bloom',
    categoria: 'Conjunto 3 piezas',
    descripcion: 'Look completo con armonía de tonos y detalles suaves, perfecto para combinar estilo, practicidad y comodidad en la rutina diaria.',
    precio: 1599,
    precioOriginal: 1999,
    esLiquidacion: true,
    fotoProducto: 'https://picsum.photos/seed/conjunto-3-bloom/600/800',
    etiqueta: 'Hot',
    departamento: 'Niña',
    tallas: ['S', 'M', 'L', 'XL'],
    colores: [
      { nombre: 'Fucsia', hex: '#ec4899' },
      { nombre: 'Lavanda', hex: '#c4b5fd' },
      { nombre: 'Blanco', hex: '#f8fafc' }
    ]
  },
  {
    id: 4,
    idProductoPrincipal: 104,
    idCategoria: 4,
    nombreProducto: 'Prenda única Lila',
    categoria: 'Prenda única',
    descripcion: 'Prenda versátil para combinar con accesorios del día a día, con un corte moderno y calidad premium en todos sus detalles.',
    precio: 899,
    esLiquidacion: false,
    fotoProducto: 'https://picsum.photos/seed/prenda-unica-lila/600/800',
    departamento: 'Niña',
    tallas: ['S', 'M', 'L'],
    colores: [
      { nombre: 'Lila', hex: '#c084fc' },
      { nombre: 'Rosa', hex: '#f9a8d4' },
      { nombre: 'Miel', hex: '#fcd34d' }
    ]
  },
  {
    id: 5,
    idProductoPrincipal: 105,
    idCategoria: 2,
    nombreProducto: 'Conjunto 2 piezas Bella',
    categoria: 'Conjunto 2 piezas',
    descripcion: 'Diseño femenino con cortes fluidos y gran comodidad, pensado para lucir con comodidad, sofisticación y fresca energía.',
    precio: 1399,
    precioOriginal: 1749,
    esLiquidacion: true,
    fotoProducto: 'https://picsum.photos/seed/conjunto-2-bella/600/800',
    etiqueta: 'Sale',
    departamento: 'Niña',
    tallas: ['S', 'M', 'L'],
    colores: [
      { nombre: 'Rosado', hex: '#f472b6' },
      { nombre: 'Salmón', hex: '#fdba74' },
      { nombre: 'Vino', hex: '#be185d' }
    ]
  },
  {
    id: 6,
    idProductoPrincipal: 106,
    idCategoria: 5,
    nombreProducto: 'Prenda única Rosé',
    categoria: 'Prenda única',
    descripcion: 'Estilo limpio y sobrio ideal para vestir con personalidad, con un acabado ligero y un diseño que se adapta a cualquier momento del día.',
    precio: 1099,
    esLiquidacion: false,
    fotoProducto: 'https://picsum.photos/seed/prenda-unica-rose/600/800',
    departamento: 'Niña',
    tallas: ['XS', 'S', 'M', 'L'],
    colores: [
      { nombre: 'Rosé', hex: '#fbcfe8' },
      { nombre: 'Durazno', hex: '#fdba74' },
      { nombre: 'Blanco', hex: '#ffffff' }
    ]
  },
  {
    id: 7,
    idProductoPrincipal: 107,
    idCategoria: 3,
    nombreProducto: 'Conjunto 3 piezas Petal',
    categoria: 'Conjunto 3 piezas',
    descripcion: 'Set completo con sensación ligera, fresca y moderna, perfecto para crear looks elegantes con una estética femenina y dinámic.',
    precio: 1799,
    precioOriginal: 2199,
    esLiquidacion: true,
    fotoProducto: 'https://picsum.photos/seed/conjunto-3-petal/600/800',
    etiqueta: 'Nuevo',
    departamento: 'Niña',
    tallas: ['S', 'M', 'L', 'XL'],
    colores: [
      { nombre: 'Petal', hex: '#fda4af' },
      { nombre: 'Mora', hex: '#d946ef' },
      { nombre: 'Blanco', hex: '#f9fafb' }
    ]
  },
  {
    id: 8,
    idProductoPrincipal: 108,
    idCategoria: 1,
    nombreProducto: 'Cartera Clásica',
    categoria: 'Cartera',
    descripcion: 'Accesorio funcional y chic para complementar cualquier look, con un diseño atemporal y excelente acabados para uso diario.',
    precio: 999,
    esLiquidacion: false,
    fotoProducto: 'https://picsum.photos/seed/cartera-clasica/600/800',
    departamento: 'Niña',
    tallas: ['S', 'M'],
    colores: [
      { nombre: 'Negro', hex: '#1f2937' },
      { nombre: 'Rosado', hex: '#f9a8d4' },
      { nombre: 'Beige', hex: '#f5f5f4' }
    ]
  }
];
