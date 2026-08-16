import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { environment } from '../../../../../environments/environment';
import type { ProductoCatalogo } from '../../interfaces/catalogo.interface';
import { CatalogoService } from '../../services/catalogo.service';
import { getColorBorderClass as getColorBorderClassUtil, getColorHex } from '../../utils/colores.util';

@Component({
  selector: 'app-detalle-producto-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './detalle-producto-page.component.html',
})
export class DetalleProductoPageComponent implements OnInit {
  producto: ProductoCatalogo | null = null;
  cantidad = 1;
  tallaSeleccionada = '';
  colorSeleccionado: { nombre: string; hex: string } | null = null;
  readonly whatsappNumero = '51918386236';
  cargando = false;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
    private readonly catalogoService: CatalogoService,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.cargando = true;

    this.catalogoService.obtenerCatalogo().subscribe({
      next: (respuesta) => {
        this.producto = (respuesta ?? []).find((item) => item.idProductoPrincipal === id) ?? (respuesta ?? [])[0] ?? null;

        if (this.producto) {
          const primeraVariante = this.producto.variantes?.[0];
          this.tallaSeleccionada = primeraVariante?.talla ?? '';
          this.colorSeleccionado = primeraVariante?.color ? { nombre: primeraVariante.color, hex: getColorHex(primeraVariante.color) } : null;
        }

        this.cargando = false;
      },
      error: () => {
        this.producto = null;
        this.cargando = false;
      },
    });
  }

  getFotoUrl(path?: string): string {
    return path ? `${environment.assetsUrl}${path}` : '';
  }

  getTallasProducto(): string[] {
    return Array.from(new Set((this.producto?.variantes ?? []).map((variante) => variante.talla).filter(Boolean)));
  }

  getColoresProducto(): Array<{ nombre: string; hex: string }> {
    const colores = (this.producto?.variantes ?? []).map((variante) => ({
      nombre: variante.color,
      hex: getColorHex(variante.color),
    }));

    return Array.from(new Map(colores.map((color) => [color.nombre, color])).values());
  }

  getColorBorderClass(colorNombre?: string): string {
    return getColorBorderClassUtil(colorNombre);
  }

  getPrecioMostrado(producto: ProductoCatalogo): number {
    return producto.precioVentaLiquidacion < producto.precioVenta ? producto.precioVentaLiquidacion : producto.precioVenta;
  }

  getPrecioTachado(producto: ProductoCatalogo): number {
    return producto.precioVentaLiquidacion < producto.precioVenta ? producto.precioVenta : 0;
  }

  disminuirCantidad(): void {
    this.cantidad = Math.max(1, this.cantidad - 1);
  }

  aumentarCantidad(): void {
    this.cantidad = this.cantidad + 1;
  }

  volverAlCatalogo(): void {
    this.router.navigate(['/']);
  }

  private formatearPrecio(precio: number): string {
    return new Intl.NumberFormat('en-US', { maximumFractionDigits: 0 }).format(precio);
  }

  consultarPorWhatsApp(): void {
    if (!this.producto) {
      return;
    }

    const talla = this.tallaSeleccionada || 'No especificada';
    const color = this.colorSeleccionado?.nombre || 'No especificado';
    const mensaje = `Hola! Quiero consultar por: ${this.producto.nombreProducto} - Talla ${talla} - Color ${color} - Cantidad: ${this.cantidad} - Precio: $${this.formatearPrecio(this.getPrecioMostrado(this.producto))}`;
    const url = `https://wa.me/${this.whatsappNumero}?text=${encodeURIComponent(mensaje)}`;

    window.open(url, '_blank', 'noopener,noreferrer');
  }
}
