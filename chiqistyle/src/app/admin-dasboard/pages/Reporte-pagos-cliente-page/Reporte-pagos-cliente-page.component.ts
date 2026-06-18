// reporte-pagos-cliente-page.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ReporteService } from '@products/services/reporte.service';
import { ClienteService } from '@products/services/cliente.service';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import { Cliente } from '@products/interfaces/cliente.interface';

// Definiciones de tipos (interfaces)
interface PagoReporte {
  idVenta: number;
  nombreCliente: string;
  fechaVenta: string;
  total: number;
}

interface DetalleProducto {
  nombreProducto: string;
  codigoSubProducto: string;
  cantidad: number;
  precioUnitario: number;
  subtotal: number;
  nombreColor: string;
  nombreTalla: string;
}

interface RegistroPago {
  idPago: number;
  monto: number;
  metodo: string;
  fechaPago: Date;
  comprobante: string;
}

@Component({
  selector: 'app-reporte-pagos-cliente-page',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './reporte-pagos-cliente-page.component.html',
})
export class ReportePagosClientePageComponent implements OnInit {
  // Data
  pagos: PagoReporte[] = []; // Ventas pagadas
  clientes: Cliente[] = [];

  // Filtros
  idCliente = 0;
  fechaInicio = '';
  fechaFin = '';

  // Variables para el Modal
  mostrarModal = false;
  ventaActualId = 0;

  // Data de los Modales
  // ✅ CORRECCIÓN CLAVE: Inicializadas como [] para evitar el error 'undefined.length'
  detalleProductos: DetalleProducto[] = [];
  registrosPagos: RegistroPago[] = [];

  constructor(
    private reporteService: ReporteService,
    private clienteService: ClienteService
  ) {}

  ngOnInit(): void {
    this.cargarClientes();
  }

  cargarClientes() {
    this.clienteService.getCliente().subscribe({
      next: (data: Cliente[]) => {
        this.clientes = data;
      },
      error: (err) => console.error('Error al cargar clientes:', err),
    });
  }
obtenerListaComprobantes(comprobante: string): string[] {
  if (!comprobante) return [];
  return comprobante.split(',');
}
  obtenerReporte() {
    const params = {
      idCliente: this.idCliente,
      fechaInicio: this.fechaInicio,
      fechaFin: this.fechaFin,
    };
    this.reporteService.obtenerPagosPorCliente(params).subscribe({
      next: (data: PagoReporte[]) => (this.pagos = data),
      error: (err) => console.error(err),
    });
  }

  totalPagos(): number {
    return this.pagos.reduce((sum, p) => sum + p.total, 0);
  }

  // --- Funcionalidad del Modal para Detalles (Pagos y Productos) ---
  abrirModalDetalles(idVenta: number) {
    this.ventaActualId = idVenta;

    // Antes de la llamada, inicializar para evitar el error de longitud en el renderizado previo
    this.detalleProductos = [];
    this.registrosPagos = [];

    this.reporteService.obtenerDetalleCompletoVenta(idVenta).subscribe({
      next: (data) => {

        console.log(data)
        // Si los datos llegan vacíos (como en tu problema anterior), se asignan arrays vacíos.
      this.detalleProductos = data.productos || []; // ✅ Asignación correcta
        this.registrosPagos = data.pagos || [];
        this.mostrarModal = true;
      },
      error: (err) => console.error('Error al obtener detalle completo:', err),
    });
  }

  cerrarModal() {
    this.mostrarModal = false;
    this.detalleProductos = [];
    this.registrosPagos = [];
    this.ventaActualId = 0;
  }

  obtenerRutaComprobante(ruta: string): string {
    const baseUrl = 'http://localhost:7038'; // ✅ Base URL Correcta
    return ruta ? `${baseUrl}${ruta}` : '#';
  }

  exportarExcel() {
    if (!this.pagos.length) return alert('No hay datos para exportar');

    const datosParaExportar = this.pagos.map(p => ({
        'Cliente': p.nombreCliente,
        'Fecha Venta': p.fechaVenta,
        'Id Venta': p.idVenta,
        'Total Venta (S/.)': p.total,
    }));

    const ws = XLSX.utils.json_to_sheet(datosParaExportar);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'ReporteVentasPagadas');
    const buffer = XLSX.write(wb, { bookType: 'xlsx', type: 'array' });
    saveAs(new Blob([buffer]), `Reporte_VentasPagadas_${new Date().toISOString().split('T')[0]}.xlsx`);
  }
}
