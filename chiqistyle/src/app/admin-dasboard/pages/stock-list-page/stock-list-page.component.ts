import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { StockItem, StockService } from '@products/services/stock.service';

@Component({
  selector: 'app-stock-list-page',
  imports: [CommonModule, FormsModule],
  templateUrl: './stock-list-page.component.html',
})
export class StockListPageComponent {
  stock: StockItem[] = [];
  stockFiltrado: StockItem[] = [];
  cargando = true;
  buscarTermino = '';
productoSeleccionado: StockItem | null = null;
  stockService = inject(StockService);

  ngOnInit(): void {
    this.cargarStock();
  }
abrirModalDetalle(item: StockItem) {
  this.productoSeleccionado = item;
}

cerrarModalDetalle() {
  this.productoSeleccionado = null;
}
  cargarStock() {
    this.stockService.listarStock().subscribe({
      next: (data) => {
        console.log(data)
        this.stock = data;
        this.stockFiltrado = data;
        this.cargando = false;
      },
      error: (e) => {
        console.error('Error al cargar stock', e);
        this.cargando = false;
      },
    });
  }

  filtrarStock() {
    // const term = this.buscarTermino.toLowerCase();
    // this.stockFiltrado = this.stock.filter(
    //   (x) =>
    //     // x.nombreProducto.toLowerCase().includes(term) ||
    //     // x.color.toLowerCase().includes(term) ||
    //     // x.talla.toLowerCase().includes(term)
    //     x.talla.toLowerCase() === term
    // );
    const term = this.buscarTermino.toLowerCase().trim();

  // si no hay texto, mostrar todo
  if (!term) {
    this.stockFiltrado = this.stock;
    return;
  }

  // búsqueda exacta por talla
  this.stockFiltrado = this.stock.filter(
    (x) => x.talla.toLowerCase() === term
  );
  }

  obtenerRutaFoto(ruta: string): string {
    return ruta ? `http://localhost:7038${ruta}` : 'assets/no-image.png';
  }
 }
