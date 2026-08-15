import { CommonModule, CurrencyPipe, NgFor, NgIf } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { PRODUCTOS_CATALOGO, type ProductoCatalogo } from '../../data/productos.mock';

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

  constructor(
    private readonly route: ActivatedRoute,
    private readonly router: Router,
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.producto = PRODUCTOS_CATALOGO.find((item) => item.id === id) ?? PRODUCTOS_CATALOGO[0];

    if (this.producto) {
      this.tallaSeleccionada = this.producto.tallas[0] ?? 'M';
      this.colorSeleccionado = this.producto.colores[0] ?? null;
    }
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

  agregarAlCarrito(): void {
    console.log('Agregar al carrito', {
      producto: this.producto?.nombreProducto,
      talla: this.tallaSeleccionada,
      color: this.colorSeleccionado?.nombre,
      cantidad: this.cantidad,
    });
  }
}
