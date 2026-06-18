import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReporteService } from '@products/services/reporte.service';
// 💡 Asumo que tienes un servicio de proveedor
import { ProveedorService } from '@products/services/proveedor.service';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';

// Puedes definir una interfaz o usar 'any' si es una estructura simple
// interface Proveedor {
//   idProveedor: number;
//   nombreProveedor: string;
// }

@Component({
  selector: 'app-reporte-compras-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './reporte-compras-page.component.html',
})
export class ReporteComprasPageComponent implements OnInit {
  // Datos principales
  compras: any[] = [];
  proveedores: any[] = []; // 💡 Declaración de proveedores

  // Variables para filtros (bindeadas con [(ngModel)])
  fechaInicio = '';
  fechaFin = '';
  idProveedor = 0;
  estado = 'Todos';

  // 💡 Inyección de servicios: ReporteService y ProveedorService
  constructor(
    private reporteService: ReporteService,
    private proveedorService: ProveedorService
  ) {}

  // 💡 Ciclo de vida para cargar datos iniciales
  ngOnInit(): void {
    this.cargarProveedores();
  }

  // --- Métodos de Lógica ---

  cargarProveedores() {
    this.proveedorService.getProveedor().subscribe({
      next: (resp) => {
        this.proveedores = resp;
      },
      error: (err) => {
        console.error('Error al cargar Proveedores:', err);
      },
    });
  }

  obtenerReporte() {
    const params = {
      fechaInicio: this.fechaInicio,
      fechaFin: this.fechaFin,
      idProveedor: this.idProveedor,
      estado: this.estado,
    };
    this.reporteService.obtenerCompras(params).subscribe({
      next: (data) => (this.compras = data),
      error: (err) => console.error(err),
    });
  }

  // 💡 Función para calcular el total de las compras, usada en el HTML
  totalCompras(): number {
    return this.compras.reduce((sum, c) => sum + c.total, 0);
  }

  exportarExcel() {
    if (!this.compras.length) return alert('No hay datos para exportar');

    // Mapeo para formatear los datos si es necesario (ej: cambiar nombres de columnas)
    const datosParaExportar = this.compras.map(c => ({
        'Proveedor': c.nombreProveedor,
        'Fecha Compra': c.fechaCompra,
        'Total (S/.)': c.total,
        'Estado': c.estado
    }));

    const ws = XLSX.utils.json_to_sheet(datosParaExportar);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Compras');
    const buffer = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });

    // Guardar el archivo
    saveAs(new Blob([buffer]), `Reporte_Compras_${new Date().toISOString().split('T')[0]}.xlsx`);
  }
}
