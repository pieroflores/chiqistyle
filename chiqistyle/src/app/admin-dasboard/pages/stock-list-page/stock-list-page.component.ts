import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { environment } from '../../../../environments/environment';
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

// Búsqueda por Nombre de Producto o Talla
    this.stockFiltrado = this.stock.filter((x) => {
      // Nos aseguramos de que no sean nulos antes de convertirlos a minúsculas
      const nombre = x.nombreProducto ? x.nombreProducto.toLowerCase() : '';
      const talla = x.talla ? String(x.talla).toLowerCase() : '';

      // Retorna true si el término de búsqueda está incluido en el nombre o en la talla
      return nombre.includes(term) || talla.includes(term);
    });
  }

  obtenerRutaFoto(ruta: string): string {
    const host = environment.apiUrl.replace(/\/api$/, '');
    return ruta ? `${host}${ruta}` : 'assets/no-image.png';
  }
 }
