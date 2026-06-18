import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import { ReporteService } from '@products/services/reporte.service';

@Component({
  selector: 'app-reporte-ventas-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reporte-ventas-page.component.html',
})
export class ReporteVentasPageComponent {
  ventas: any[] = [];
  fechaInicio: string = '';
  fechaFin: string = '';
  estado: string = 'Todos';
  metodoPago: string = 'Todos';

  constructor(private reporteService: ReporteService) {}

  obtenerReporte() {
    const params = { fechaInicio: this.fechaInicio, fechaFin: this.fechaFin, estado: this.estado, metodoPago: this.metodoPago };
    this.reporteService.obtenerVentas(params).subscribe({

       next: (data) => {
      console.log('VENTAS =>', data);   // 👈 aquí ves todo lo que llega
      this.ventas = data;
    },
      error: (e) => console.error('Error al obtener reporte', e),

    });
  }

  cambiarEnvio(v: any) {
  const nuevoValor = !v.enviado;

  this.reporteService.actualizarEnvio(v.idVenta, nuevoValor).subscribe({
    next: () => {
      v.enviado = nuevoValor;

      // 🔥 OPCIONAL: cambiar estado automático
      if (nuevoValor && v.estado === 'Pagado') {
        v.estado = 'Enviado';
      }
    },
    error: (err) => {
      console.error(err);
      alert('Error al actualizar envío');
    }
  });
}
  totalGlobal() { return this.ventas.reduce((s, v) => s + (v.total || 0), 0); }
  totalPagado() { return this.ventas.reduce((s, v) => s + (v.montoPagado || 0), 0); }
  totalPendiente() { return this.ventas.reduce((s, v) => s + ((v.total || 0) - (v.montoPagado || 0)), 0); }
  totalCompras() {  return this.ventas.reduce((s, v) => s + (v.precioCompraTotal || 0), 0);  }
  totalUtilidad() {  return this.totalPagado() - this.totalCompras(); }

  exportarExcel() {
    if (!this.ventas.length) return alert('No hay datos para exportar');
    const ws = XLSX.utils.json_to_sheet(this.ventas);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Ventas');
    const buffer = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
    saveAs(new Blob([buffer]), `Reporte_Ventas_${new Date().toISOString().split('T')[0]}.xlsx`);
  }
}
