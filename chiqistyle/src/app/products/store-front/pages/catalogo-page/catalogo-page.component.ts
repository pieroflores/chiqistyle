import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { environment } from '../../../../../environments/environment';
import type { ProductoCatalogo } from '../../interfaces/catalogo.interface';
import { CatalogoService } from '../../services/catalogo.service';

@Component({
  selector: 'app-catalogo-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './catalogo-page.component.html',
})
export class CatalogoPageComponent implements OnInit {
  readonly departamentos = [
    { nombre: 'Niña', activo: true },
    { nombre: 'Niño', activo: false, proximamente: true },
    { nombre: 'Mujer', activo: false, proximamente: true }
  ];

  productos: ProductoCatalogo[] = [];
  categorias: string[] = ['Todos'];
  cargando = false;

  constructor(private readonly catalogoService: CatalogoService) {}

  ngOnInit(): void {
    this.cargando = true;

    this.catalogoService.obtenerCatalogo().subscribe({
      next: (respuesta) => {
        this.productos = respuesta ?? [];
        this.categorias = ['Todos', ...Array.from(new Set(this.productos.map((producto) => producto.categoria).filter(Boolean)))];
        this.cargando = false;
      },
      error: () => {
        this.productos = [];
        this.categorias = ['Todos'];
        this.cargando = false;
      },
    });
  }

  getFotoUrl(path?: string): string {
    return path ? `${environment.assetsUrl}${path}` : '';
  }

  getPrecioMostrado(producto: ProductoCatalogo): number {
    return producto.precioVentaLiquidacion < producto.precioVenta ? producto.precioVentaLiquidacion : producto.precioVenta;
  }

  getPrecioTachado(producto: ProductoCatalogo): number {
    return producto.precioVentaLiquidacion < producto.precioVenta ? producto.precioVenta : 0;
  }

  esLiquidacion(producto: ProductoCatalogo): boolean {
    return producto.precioVentaLiquidacion < producto.precioVenta;
  }

  getEtiqueta(producto: ProductoCatalogo): string {
    return this.esLiquidacion(producto) ? 'Liquidación' : '';
  }

  abrirWhatsAppGeneral(): void {
    const mensaje = 'Hola! Quiero consultar sobre productos de Chiqistyle.';
    const url = `https://wa.me/51918386236?text=${encodeURIComponent(mensaje)}`;
    window.open(url, '_blank', 'noopener,noreferrer');
  }

  abrirWhatsAppProducto(producto: ProductoCatalogo): void {
    const precio = this.getPrecioMostrado(producto);
    const mensaje = `Hola! Quiero consultar por: ${producto.nombreProducto} - Precio: $${precio.toLocaleString('en-US')}`;
    const url = `https://wa.me/51918386236?text=${encodeURIComponent(mensaje)}`;
    window.open(url, '_blank', 'noopener,noreferrer');
  }

  getBadgeClasses(etiqueta?: string): string {
    const base = 'inline-flex items-center rounded-full border px-2.5 py-1 text-[10px] font-bold uppercase tracking-[0.18em]';

    if (!etiqueta) {
      return `${base} border-transparent bg-transparent text-transparent`;
    }

    switch (etiqueta) {
      case 'Liquidación':
        return `${base} border-pink-200 bg-pink-100 text-pink-700`;
      default:
        return `${base} border-pink-200 bg-pink-100 text-pink-700`;
    }
  }
}
