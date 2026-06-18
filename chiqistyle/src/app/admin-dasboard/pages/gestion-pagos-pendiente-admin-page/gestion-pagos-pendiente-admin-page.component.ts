import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ClienteDeuda, VentaPendiente } from '@products/interfaces/venta.interface';
import { VentaService } from '@products/services/venta.service';
import Swal from 'sweetalert2';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';

// ─── Métodos TESMA: NO generan PDF automático ─────────────────
const METODOS_TESMA = [
  'YAPE TESMA',
  'TRANSFERENCIA BCP TESMA',
  'PLIN TESMA',
  'TRANSFERENCIA BBVA TESMA',
];

@Component({
  selector: 'app-gestion-pagos-pendiente-admin-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './gestion-pagos-pendiente-admin-page.component.html',
})
export class GestionPagosPendienteAdminPageComponent implements OnInit {

  ventaService = inject(VentaService);

  // ─── Datos principales ────────────────────────────────────────
  clientesDeuda: ClienteDeuda[] = [];
  ventasPendientes: VentaPendiente[] = [];
  ventaSeleccionada: VentaPendiente | null = null;
  detalleVenta: any[] = [];
  pagosVenta: any[] = [];
  totalDeudaCliente: number = 0;

  // ─── Estados UI ───────────────────────────────────────────────
  cargando = true;
  cargandoDetalle = false;
  guardandoPago = false;
  modalDetalleVisible = false;
  modalPagoVisible = false;

  // ─── Buscador ─────────────────────────────────────────────────
  filtroBusqueda: string = '';

  // ─── Datos del pago grupal ────────────────────────────────────
  montoAdicional: number = 0;
  metodoPago: string = '';
  comprobanteFiles: File[] = [];
  comprobantePreviews: string[] = [];

  // ─── Para PDF ─────────────────────────────────────────────────
  ventaParaPdf: VentaPendiente | null = null;
  detalleParaPdf: any[] = [];

  // =============================================================
  // LIFECYCLE
  // =============================================================

  ngOnInit(): void {
    this.obtenerClientesDeuda();
  }

  // =============================================================
  // BUSCADOR
  // =============================================================

  get clientesFiltrados(): ClienteDeuda[] {
    if (!this.filtroBusqueda.trim()) return this.clientesDeuda;
    const texto = this.filtroBusqueda.toLowerCase().trim();
    return this.clientesDeuda.filter(c =>
      c.nombreCliente.toLowerCase().includes(texto) ||
      c.dni.includes(texto)
    );
  }

  // =============================================================
  // CARGA DE DATOS
  // =============================================================

  obtenerClientesDeuda() {
    this.cargando = true;
    this.ventaService.getClientesDeuda().subscribe({
      next: (data) => {
        this.clientesDeuda = data;
        this.cargando = false;
      },
      error: (err) => {
        console.error(err);
        this.cargando = false;
      }
    });
  }

  // =============================================================
  // MODAL DETALLE CLIENTE
  // =============================================================

  abrirDetalleCliente(cliente: any) {
    this.ventaService.listarPendientes().subscribe(data => {
      this.ventasPendientes = data.filter(
        v => v.nombreCliente === cliente.nombreCliente
      );

      if (this.ventasPendientes.length === 0) {
        Swal.fire('Aviso', 'Este cliente no tiene ventas pendientes', 'info');
        return;
      }

      this.totalDeudaCliente = this.ventasPendientes
        .reduce((total, venta) => total + venta.montoPendiente, 0);

      this.modalDetalleVisible = true;
      this.abrirDetalle(this.ventasPendientes[0]);
    });
  }

  abrirDetalle(venta: VentaPendiente) {
    this.ventaSeleccionada = venta;
    this.cargandoDetalle = true;
    this.detalleVenta = [];
    this.pagosVenta = [];

    this.ventaService.obtenerDetalleVenta(venta.idVenta).subscribe({
      next: (data) => {
        this.detalleVenta = data;
        this.cargandoDetalle = false;
      },
      error: () => { this.cargandoDetalle = false; }
    });

    this.ventaService.listarPagosPorVenta(venta.idVenta).subscribe({
      next: (data) => { this.pagosVenta = data; }
    });
  }

  cerrarModalDetalle() {
    this.modalDetalleVisible = false;
    this.ventaSeleccionada = null;
    this.ventasPendientes = [];
    this.detalleVenta = [];
    this.pagosVenta = [];
    this.totalDeudaCliente = 0;
  }

  // =============================================================
  // COMPROBANTES DE LA VENTA ORIGINAL
  // =============================================================

  obtenerComprobantesVentaOriginal(): string[] {
    if (!this.ventaSeleccionada?.comprobante) return [];
    return this.obtenerListaComprobantes(this.ventaSeleccionada.comprobante);
  }

  // =============================================================
  // MODAL PAGO GRUPAL
  // =============================================================

  abrirModalPago() {
    this.montoAdicional = this.totalDeudaCliente;
    this.metodoPago = '';
    this.comprobanteFiles = [];
    this.comprobantePreviews = [];
    this.modalPagoVisible = true;
  }

  cerrarModalPago() {
    this.modalPagoVisible = false;
    this.comprobanteFiles = [];
    this.comprobantePreviews = [];
    this.guardandoPago = false;
  }

  // =============================================================
  // MANEJO DE ARCHIVOS
  // =============================================================

  // onFilesSelected(event: any) {
  //   const files: FileList = event.target.files;
  //   if (!files || files.length === 0) return;
  //   for (let i = 0; i < files.length; i++) {
  //     const file = files[i];
  //     this.comprobanteFiles.push(file);
  //     const reader = new FileReader();
  //     reader.onload = (e: any) => { this.comprobantePreviews.push(e.target.result); };
  //     reader.readAsDataURL(file);
  //   }
  // }
  onFilesSelected(event:any){

const files:FileList = event.target.files;


if(!files || files.length===0){
return;
}



for(let i=0;i<files.length;i++){

const file = files[i];


if(!file.type.startsWith('image')){
continue;
}


// guardar archivo real
this.comprobanteFiles.push(file);



const reader = new FileReader();


reader.onload=(e:any)=>{

this.comprobantePreviews.push(
e.target.result
);

};


reader.readAsDataURL(file);


}


// limpiar input
event.target.value='';


}
pegarImagen(event:ClipboardEvent){


const items =
event.clipboardData?.items;


if(!items){
return;
}



for(let i=0;i<items.length;i++){


const item = items[i];



if(item.type.startsWith('image')){


const blob=item.getAsFile();



if(blob){



const archivo = new File(
[blob],
`comprobante_${Date.now()}_${i}.png`,
{
type:blob.type
}
);



this.comprobanteFiles.push(archivo);



const reader = new FileReader();



reader.onload=(e:any)=>{


this.comprobantePreviews.push(
e.target.result
);


};



reader.readAsDataURL(archivo);



}


}



}


}

  eliminarComprobante(index: number) {
    this.comprobanteFiles.splice(index, 1);
    this.comprobantePreviews.splice(index, 1);
  }

  // =============================================================
  // REGISTRAR PAGO GRUPAL
  // =============================================================

  registrarPagoGrupal() {
    if (!this.montoAdicional || this.montoAdicional <= 0) {
      Swal.fire('Atención', 'Ingresa un monto válido', 'warning');
      return;
    }
    if (!this.metodoPago) {
      Swal.fire('Atención', 'Selecciona un método de pago', 'warning');
      return;
    }
    const idCliente = this.ventasPendientes[0]?.idCliente;
    if (!idCliente) {
      Swal.fire('Error', 'No se encontró el cliente', 'error');
      return;
    }

    this.guardandoPago = true;

    if (this.comprobanteFiles.length === 0) {
      this.ejecutarPagoGrupal(idCliente, null);
      return;
    }

    const uploads = this.comprobanteFiles.map(f =>
      this.ventaService.uploadComprobante(f).toPromise()
    );

    Promise.all(uploads)
      .then((results: any[]) => {
        const rutas = results.map((r: any) => r.path).join(',');
        this.ejecutarPagoGrupal(idCliente, rutas);
      })
      .catch((err) => {
        console.error(err);
        this.guardandoPago = false;
        Swal.fire('Error', 'No se pudieron subir los comprobantes', 'error');
      });
  }

  private ejecutarPagoGrupal(idCliente: number, comprobante: string | null) {
    const data = { idCliente, monto: this.montoAdicional, metodo: this.metodoPago, comprobante };

    // ✅ GUARDAR SNAPSHOT antes de que los modales se cierren y limpien los datos
    const ventasSnapshot     = [...this.ventasPendientes];
    const montoSnapshot      = this.montoAdicional;
    const metodoSnapshot     = this.metodoPago;
    const totalDeudaSnapshot = this.totalDeudaCliente;

    this.ventaService.pagarCliente(data).subscribe({
      next: () => {
        this.guardandoPago = false;

        const esPagoCompleto = montoSnapshot >= totalDeudaSnapshot;
        const esMetodoTesma  = METODOS_TESMA.includes(metodoSnapshot);

        // ✅ Primero cerrar modales y refrescar lista
        this.cerrarModalPago();
        this.cerrarModalDetalle();
        this.obtenerClientesDeuda();

        // ✅ Luego generar PDF usando el snapshot (ya no depende del estado del componente)
        if (esPagoCompleto && !esMetodoTesma) {
          this.generarPdfConDatos(ventasSnapshot, montoSnapshot, metodoSnapshot);
        }

        Swal.fire('✅ Éxito', 'Pago registrado y distribuido correctamente', 'success');
      },
      error: (err) => {
        console.error(err);
        this.guardandoPago = false;
        Swal.fire('Error', 'No se pudo registrar el pago', 'error');
      }
    });
  }

  // =============================================================
  // GENERAR PDF usando datos pasados por parámetro (snapshot)
  // así no depende del estado del componente que ya fue limpiado
  // =============================================================

  private generarPdfConDatos(
    ventas: VentaPendiente[],
    monto: number,
    metodo: string
  ) {
    const cliente = ventas[0];
    if (!cliente) return;

    // Cargar productos de TODAS las ventas en paralelo
    const peticiones = ventas.map(v =>
      this.ventaService.obtenerDetalleVenta(v.idVenta).toPromise()
    );

    Promise.all(peticiones)
      .then((resultados: any[]) => {
        let totalProductos = 0;

        resultados.forEach((productos: any[]) => {
          productos.forEach(p => {
            totalProductos += Number(p.precioUnitario) * Number(p.cantidad);
          });
        });
        const doc = new jsPDF('p', 'mm', 'a4');
        let y = 15;

        // ── Encabezado ───────────────────────────────────────────
        doc.setFontSize(16);
        doc.setFont('helvetica', 'bold');
        doc.text('COMPROBANTE DE PAGO', 105, y, { align: 'center' });
        y += 7;

        doc.setFontSize(10);
        doc.setFont('helvetica', 'normal');
        doc.text(`Fecha: ${new Date().toLocaleDateString('es-PE')}`, 105, y, { align: 'center' });
        y += 12;

        // ── Datos del cliente ────────────────────────────────────
        doc.setFontSize(11);
        doc.setFont('helvetica', 'bold');
        doc.text('Datos del Cliente', 14, y);
        y += 6;

        doc.setFont('helvetica', 'normal');
        doc.setFontSize(10);
        doc.text(`Cliente     : ${cliente.nombreCliente}`, 14, y); y += 5;
        doc.text(`DNI         : ${cliente.dni ?? '—'}`, 14, y);    y += 5;
        doc.text(`Método Pago : ${metodo}`, 14, y);                y += 5;
        doc.text(`Total Pagado: S/. ${totalProductos.toFixed(2)}`, 14, y);  y += 12;

        // ── Tabla resumen ventas ─────────────────────────────────
        doc.setFontSize(11);
        doc.setFont('helvetica', 'bold');
        doc.text('Ventas Canceladas', 14, y);
        y += 4;

        autoTable(doc, {
          startY: y,
          head: [['Venta', 'Fecha', 'Total', 'Ya Pagado', 'Saldo Cancelado']],
          body: ventas.map(v => [
            `#${v.idVenta}`,
            new Date(v.fechaVenta).toLocaleDateString('es-PE'),
            `S/. ${Number(v.total).toFixed(2)}`,
            `S/. ${Number(v.montoPagado).toFixed(2)}`,
            `S/. ${Number(v.montoPendiente).toFixed(2)}`,
          ]),
          styles: { fontSize: 9 },
          headStyles: { fillColor: [41, 128, 185] },
          alternateRowStyles: { fillColor: [235, 245, 255] },
        });

        y = (doc as any).lastAutoTable.finalY + 12;

        // ── Productos de CADA venta ──────────────────────────────
        ventas.forEach((venta, index) => {
          const productos: any[] = resultados[index] ?? [];
          if (productos.length === 0) return;

          if (y > 240) { doc.addPage(); y = 15; }

          doc.setFontSize(11);
          doc.setFont('helvetica', 'bold');
          doc.setTextColor(0, 0, 0);
          doc.text(`Productos — Venta #${venta.idVenta}`, 14, y);
          y += 4;

          autoTable(doc, {
            startY: y,
            head: [['Producto', 'Código', 'Cant.', 'Precio Unit.', 'Subtotal']],
            body: productos.map((item: any) => [
              item.nombreProducto,
              item.codigoSubProducto ?? '—',
              String(item.cantidad),
              `S/. ${Number(item.precioUnitario).toFixed(2)}`,
              `S/. ${Number(item.subtotal).toFixed(2)}`,
            ]),
            styles: { fontSize: 9 },
            headStyles: { fillColor: [39, 174, 96] },
            alternateRowStyles: { fillColor: [235, 255, 240] },
          });

          y = (doc as any).lastAutoTable.finalY + 10;
        });

        // ── Total final ──────────────────────────────────────────
        if (y > 260) { doc.addPage(); y = 15; }

        doc.setDrawColor(200, 200, 200);
        doc.line(14, y, 196, y);
        y += 6;

        doc.setFontSize(13);
        doc.setFont('helvetica', 'bold');
        doc.setTextColor(0, 0, 0);
        doc.text(
          `TOTAL CANCELADO: S/. ${totalProductos.toFixed(2)}`,
          105, y, { align: 'center' }
        );
        y += 8;

        doc.setFontSize(9);
        doc.setFont('helvetica', 'italic');
        doc.setTextColor(120, 120, 120);
        doc.text('Gracias por su pago.', 105, y, { align: 'center' });

        doc.save(`comprobante_${cliente.nombreCliente.replace(/ /g, '_')}_${Date.now()}.pdf`);
      })
      .catch(err => {
        console.error('Error al generar PDF', err);
      });
  }

  // =============================================================
  // UTILIDADES
  // =============================================================

  obtenerRutaComprobante(ruta: string): string {
    if (!ruta || ruta.trim() === '') return '';
    if (ruta.startsWith('http')) return ruta;
    return `http://localhost:7038${ruta}`;
  }

  obtenerListaComprobantes(comprobante: string): string[] {
    if (!comprobante) return [];
    return comprobante.split(',').map(r => r.trim()).filter(r => r.length > 0);
  }

  esImagen(ruta: string): boolean {
    if (!ruta) return false;
    const ext = ruta.toLowerCase().split('.').pop();
    return ['jpg', 'jpeg', 'png', 'gif', 'webp', 'bmp'].includes(ext ?? '');
  }
}
