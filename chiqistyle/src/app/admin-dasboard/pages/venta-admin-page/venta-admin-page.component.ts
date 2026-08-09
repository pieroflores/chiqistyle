import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { Cliente } from '@products/interfaces/cliente.interface';
import { MetodoPago, ProductoDisponibles } from '@products/interfaces/compra.interface';
import { ProductoVentaInfo as MConsultaProductoVenta, listprodMostrar, VentaEnviar, VentaProductoEnviar } from '@products/interfaces/venta.interface';
import { ClienteService } from '@products/services/cliente.service';
import { CompraService } from '@products/services/compra.service';
import { VentaService } from '@products/services/venta.service';
import Swal from 'sweetalert2';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';
import { environment } from '../../../../environments/environment';


@Component({
  selector: 'app-venta-admin-page',
  standalone: true,
  imports: [CommonModule,FormsModule,NgSelectModule],
  templateUrl: './venta-admin-page.component.html',
  styleUrls: ['./venta-admin-page.component.scss']
})
export class VentaAdminPageComponent implements OnInit{
  apiHost = environment.apiUrl.replace(/\/api$/, '');
  isDragging = false;
 
  // 🔹 PROPIEDADES DE PRECIOS
  public usaPrecioPorMayor: number = 0; // Estado del selector: false=Normal, true=PorMayor
public tipoPrecioSeleccionado:number =0;
  public precioVentaPorMayor: number = 0;
  public precioVentaNormal: number = 0;
public precioVentaLiquidacion: number= 0;

  compraService = inject(CompraService)
  ClienteService= inject(ClienteService);
  productoDisponibles: ProductoDisponibles[]=[]
  clientes: Cliente[] = [];
  cantidad: number = 0;
  idProductoSeleccionado: number | null = null;
  idClienteSeleccionado: number | null = null;
  nombreVendedor: string = '';
  idUsuario: number | null = null;
  ventaService = inject(VentaService);
  precioVenta: number | null = null; // Precio que se muestra y se envía
  ubicacionTexto: string = '';
  stockDisponible: number | null = null;
  // Usamos idAlmacenSeleccionado
  idAlmacenSeleccionado: number | null = null;

  VentaList: listprodMostrar[] = [];
  totalVenta: number = 0;
  montoPagado: number= 0;
 selectedFiles: File[] = [];
  previewUrls: (string | ArrayBuffer)[] = [];
  fechaventa: Date | null = new Date();
  metodoPago : string='';
  tipoTransaccion: string='';
  comprobantePath: string | null = null;

mostrarModalCliente = false;

nuevoCliente: any = {
  nombreCliente: '',
  dni: '',
  telefono: '',
  direccion: ''
};

textoBusquedaCliente: string = '';
metodosPago: MetodoPago[] = [];

  ngOnInit(): void {
     const userData = localStorage.getItem('usuario');
      if (userData) {
        const usuario = JSON.parse(userData);
        this.nombreVendedor = usuario.nombreCompleto;
        this.idUsuario = usuario.idUsuario
      }
      this.cargarCombos();
      this.cargarCliente();
      this.fechaventa = new Date(); // Inicializa la fecha al día de hoy
  }
 
  calcularTotal() {
    this.totalVenta = this.VentaList.reduce((acc, item) => {
      // Asegura que precioUnitario y cantidad sean números
      return acc + (item.cantidad * item.precioUnitario);
    }, 0);
  }

onBuscarCliente(event: any) {
  this.textoBusquedaCliente = event.term;
}
abrirModalCliente(search: string) {
  this.textoBusquedaCliente = search;

  this.nuevoCliente = {
    nombreCliente: search, // 👈 se llena automático
    dni: '',
    telefono: ''
  };

  this.mostrarModalCliente = true;
}
  onProductoSeleccionado() {
    if (this.idProductoSeleccionado) {
        // Usamos ProductoVentaInfo (aliased as MConsultaProductoVenta) del servicio
        this.ventaService.obtenerInfoProducto(this.idProductoSeleccionado).subscribe({
            next: (data: MConsultaProductoVenta[]) => {
                if (data.length > 0) {
                    const productoInfo = data[0];
console.log(productoInfo)
                    this.stockDisponible = productoInfo.cantidadDisponible;
                    this.ubicacionTexto = productoInfo.ubicacionTexto;
                    this.idAlmacenSeleccionado = productoInfo.idAlmacen;

                    // 🔹 GUARDAR AMBOS PRECIOS (ambos existen en ProductoVentaInfo)
                    this.precioVentaNormal = productoInfo.precioVenta;
                    this.precioVentaLiquidacion= productoInfo.precioVentaLiquidacion;
                    this.precioVentaPorMayor = productoInfo.precioVentaPorMayor;

                    // 🔹 INICIALIZAR EL PRECIO UNITARIO MOSTRADO (usando el valor actual de usaPrecioPorMayor)
                    this.actualizarPrecioVenta();
                } else {
                  Swal.fire('Error', 'No se encontró información del producto.', 'error');
                  this.resetearDatosProducto();
                }
            },
            error: (err) => {
              console.error('Error al obtener info del producto:', err);
              Swal.fire('Error', 'Fallo la conexión al obtener la información del producto.', 'error');
              this.resetearDatosProducto();
            }
        });
    } else {
        this.resetearDatosProducto();
    }
  }

  /** 🔹 Alterna entre precio normal y precio por mayor al cambiar el checkbox */
//   actualizarPrecioVenta() {
//     if (this.usaPrecioPorMayor) {
//         this.precioVenta = this.precioVentaPorMayor; // Mostrar precio por mayor
//     } else {
//         this.precioVenta = this.precioVentaNormal; // Mostrar precio normal
//     }
//   }
 actualizarPrecioVenta(){

  if(this.usaPrecioPorMayor == 0){

    this.precioVenta = this.precioVentaNormal;

  }
  else if(this.usaPrecioPorMayor == 1){

    this.precioVenta = this.precioVentaPorMayor;

  }
  else if(this.usaPrecioPorMayor == 2){

    this.precioVenta = this.precioVentaLiquidacion;

  }
  else if(this.usaPrecioPorMayor == 3){

    // Precio Especial
    // dejamos que el usuario escriba
    this.precioVenta = 0;

  }

}
buscarCliente(term: string, item: any): boolean {
  if (!term) return true; // Si no hay texto, muestra todos

  term = term.toLowerCase();

  const nombre = item.nombreCliente?.toLowerCase() ?? '';
  const telefono = item.telefono ? item.telefono.toString() : '';

  // Retorna true si el término está en el nombre o teléfono
  return nombre.includes(term) || telefono.includes(term);
}
buscarProducto(term: string, item: ProductoDisponibles): boolean {
  if (!term) return true; // Si el usuario no escribe nada, muestra todo
  term = term.toLowerCase();

  const nombre = item.productoVariable?.toLowerCase() ?? '';

  return nombre.includes(term);
}
  agregarDetalle() {

    // Validación de campos esenciales
    if (!this.idClienteSeleccionado || !this.idProductoSeleccionado  || !this.fechaventa || !this.cantidad) {
      Swal.fire({
        icon: 'warning',
        title: 'Campos incompletos',
        text: 'Debes seleccionar cliente, producto, cantidad y fecha.',
      });
      return;
    }

    if (!this.precioVenta || this.precioVenta <= 0 || !this.idAlmacenSeleccionado) {
      Swal.fire({ icon: 'warning', title: 'Datos Faltantes', text: 'El producto no tiene precio o ubicación válida. Intenta seleccionar de nuevo.' });
      return;
    }
   
    const producto = this.productoDisponibles.find(p => p.idSubProducto === this.idProductoSeleccionado);
    const cliente = this.clientes.find(p => p.idCliente === this.idClienteSeleccionado);

    if (!producto || !cliente) {
      Swal.fire({ icon: 'error', title: 'Error de Datos', text: 'Producto o Cliente no encontrado.' });
      return;
    }

    const stockDisponible = this.stockDisponible;
    if (this.cantidad > stockDisponible!) {
      Swal.fire({
        icon: 'error',
        title: 'Stock insuficiente',
        text: `Solo hay ${stockDisponible} unidades disponibles en el almacén.`,
        confirmButtonColor: '#d33'
      });
      return;
    }

    // DETALLE PARA MOSTRAR EN LA TABLA (incluye datos de envío para mapeo posterior)
    const nuevoMostrar : listprodMostrar ={
      cliente: cliente.nombreCliente , //+ ' - '+cliente.telefono,
      fechaVenta: this.fechaventa,
      SubProducto: producto.productoVariable,
      cantidad: this.cantidad,
      precioUnitario: this.precioVenta,
      almacen: this.ubicacionTexto,
      usuario: this.nombreVendedor,
     
      // 🔹 PROPIEDADES DE ENVÍO REQUERIDAS (gracias a que existen en listprodMostrar)
      idSubProducto: this.idProductoSeleccionado!,
      idAlmacen: this.idAlmacenSeleccionado!,
      usaPrecioPorMayor: this.usaPrecioPorMayor
    };

    this.VentaList.push(nuevoMostrar);

    this.calcularTotal();
    this.resetearDatosProducto(); // Limpiar campos de producto
   
    Swal.fire({
      icon: 'success',
      title: 'Producto agregado',
      text: 'El producto fue añadido correctamente al pedido.',
      showConfirmButton: false,
      timer: 1200
    });
  }

  resetearDatosProducto() {
    this.idProductoSeleccionado = null;
    this.cantidad = 0;
    this.precioVenta = null;
    this.ubicacionTexto = '';
    this.stockDisponible = null;
    this.idAlmacenSeleccionado = null;
    this.precioVentaNormal = 0;
    this.precioVentaPorMayor = 0;
    this.usaPrecioPorMayor = 0; // Resetear el selector de precio a "Precio Normal"
  }

  // --- Funciones de carga y manejo de archivos ---

  cargarCombos() {
    this.compraService.cargarCombos().subscribe({
      next: (resp) => {
  console.log(resp)
        this.productoDisponibles = resp.productoDisponibles;
this.metodosPago = resp.metodosPago;
      },
      error: (err) => {
        console.error('Error al cargar Combos:', err);
      }
    });
  }
  cargarCliente() {
    this.ClienteService.getCliente().subscribe({
      next: (resp) => {
        this.clientes = resp;
      },
      error: (err) => {
        console.error('Error al cargar Cliente:', err);
      }
      });
  }
  eliminarDetalle(index: number) {
     this.VentaList.splice(index, 1);
     this.calcularTotal();
      Swal.fire({
         icon: 'info',
         title: 'Producto eliminado',
         text: 'El producto fue quitado del detalle.',
         showConfirmButton: false,
         timer: 1200
       });
  }
  resetFormularioVenta() {
    this.idClienteSeleccionado = null;
    this.fechaventa = new Date();
    this.montoPagado = 0;
    this.metodoPago = '';
    this.tipoTransaccion = '';
   this.previewUrls = [];
  this.selectedFiles = [];
    this.totalVenta = 0;
    this.VentaList = [];
    this.resetearDatosProducto();
    this.comprobantePath = null;
  }
  onDragOver(event: DragEvent) {
    event.preventDefault();
    this.isDragging = true;
  }
  onDragLeave(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;
  }
  onFileDrop(event: DragEvent) {
    event.preventDefault();
    this.isDragging = false;

    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
        const files = event.dataTransfer.files;
       for (let i = 0; i < files.length; i++) {
        this.handleImageFile(files[i]);
}
    }
  }
  onPasteImage(event: ClipboardEvent) {
    const items = event.clipboardData?.items;
    if (!items) return;

    for (let i = 0; i < items.length; i++) {
        const item = items[i];
        if (item.type.indexOf("image") !== -1) {
            const blob = item.getAsFile();
            if (blob) {
                this.handleImageFile(blob);
            }
        }
    }
  }
  private handleImageFile(file: File) {
   this.selectedFiles.push(file);

  const reader = new FileReader();

  reader.onload = () => {
    this.previewUrls.push(reader.result!);
  };

  reader.readAsDataURL(file);
  }
  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;

  if (input.files && input.files.length > 0) {

    for (let i = 0; i < input.files.length; i++) {
      this.handleImageFile(input.files[i]);
    }

  }
  }

  // --- Funciones de registro de venta ---
eliminarImagen(index: number) {

  this.selectedFiles.splice(index, 1);
  this.previewUrls.splice(index, 1);

}
  /*agregarVenta(){
    if (!this.VentaList.length) {
      Swal.fire('Atención', 'No hay productos en el pedido', 'warning');
      return;
    }
    if (!this.idClienteSeleccionado || !this.fechaventa || !this.idUsuario) {
      Swal.fire('Atención', 'Faltan datos del Cliente o Vendedor', 'warning');
      return;
    }

    if (!this.tipoTransaccion || !this.montoPagado || !this.metodoPago || this.selectedFiles.length === 0){
      let faltantes: string[] = [];
      if (!this.tipoTransaccion) faltantes.push('Tipo de Transacción');
      if (!this.montoPagado) faltantes.push('Monto Pagado / Adelanto');
      if (!this.metodoPago) faltantes.push('Método de Pago');
      if (this.selectedFiles.length === 0) faltantes.push('Foto del Comprobante');

      Swal.fire({
        icon: 'warning',
        title: 'Campos incompletos',
        html: `<b>Faltan completar los siguientes campos:</b><br><ul class="text-left mt-2">${faltantes
          .map(f => `<li>• ${f}</li>`)
          .join('')}</ul>`,
        confirmButtonColor: '#d33'
      });
      return;
    }

    const detalleParaEnviar: VentaProductoEnviar[] = this.VentaList.map(item => ({
      idSubProducto: item.idSubProducto!,
      cantidad: item.cantidad,
      precioUnitario: item.precioUnitario,
      idAlmacen: item.idAlmacen!,
      usaPrecioPorMayor: item.usaPrecioPorMayor!
    }));

    const nuevoEnviarServicio: VentaEnviar = {
      idCliente: this.idClienteSeleccionado!,
      fechaVenta: this.fechaventa!,
      idUsuario: this.idUsuario!,
      detalle: detalleParaEnviar,
      montoPagado: this.montoPagado,
      metodoPago: this.metodoPago,
      comprobante: '',
      tipoTransaccion: this.tipoTransaccion
    }

    if (this.selectedFiles.length > 0) {
      const uploads = this.selectedFiles.map(file =>
  this.ventaService.uploadComprobante(file).toPromise()
);

Promise.all(uploads).then((results:any) => {

  const paths = results.map((r:any) => r.path);

  nuevoEnviarServicio.comprobante = paths.join(',');

  this.enviarVenta(nuevoEnviarServicio);

}).catch(() => {

  Swal.fire('❌ Error', 'No se pudieron subir los comprobantes', 'error');

});;
    } else {
     
      this.enviarVenta(nuevoEnviarServicio);
    }

 if (
    this.tipoTransaccion === 'COMPLETA' &&
    this.metodoPago !== 'YAPE TESMA' &&  this.metodoPago !== 'TRANSFERENCIA BCP TESMA'
    &&  this.metodoPago !== 'PLIN TESMA' &&  this.metodoPago !== 'TRANSFERENCIA BBVA TESMA'
  ) {
    this.generarPDFComprobante();
  }

  } */
agregarVenta() {
    if (!this.VentaList.length) {
      Swal.fire('Atención', 'No hay productos en el pedido', 'warning');
      return;
    }
    if (!this.idClienteSeleccionado || !this.fechaventa || !this.idUsuario) {
      Swal.fire('Atención', 'Faltan datos del Cliente o Vendedor', 'warning');
      return;
    }

    // Validación de campos de pago
    if (!this.tipoTransaccion || !this.montoPagado || !this.metodoPago || this.selectedFiles.length === 0){
      let faltantes: string[] = [];
      if (!this.tipoTransaccion) faltantes.push('Tipo de Transacción');
      if (!this.montoPagado) faltantes.push('Monto Pagado / Adelanto');
      if (!this.metodoPago) faltantes.push('Método de Pago');
      if (this.selectedFiles.length === 0) faltantes.push('Foto del Comprobante');

      Swal.fire({
        icon: 'warning',
        title: 'Campos incompletos',
        html: `<b>Faltan completar los siguientes campos:</b><br><ul class="text-left mt-2">${faltantes
          .map(f => `<li>• ${f}</li>`)
          .join('')}</ul>`,
        confirmButtonColor: '#d33'
      });
      return;
    }

    // 🔹 Mapear la VentaList (visual) al formato de envío (VentaProductoEnviar)
    const detalleParaEnviar: VentaProductoEnviar[] = this.VentaList.map(item => ({
      idSubProducto: item.idSubProducto!,
      cantidad: item.cantidad,
      precioUnitario: item.precioUnitario,
      idAlmacen: item.idAlmacen!,
      usaPrecioPorMayor: item.usaPrecioPorMayor! // 👈 Incluye el indicador
    }));

    const nuevoEnviarServicio: VentaEnviar = {
      idCliente: this.idClienteSeleccionado!,
      fechaVenta: this.fechaventa!,
      idUsuario: this.idUsuario!,
      detalle: detalleParaEnviar, // 👈 Usamos el detalle mapeado
      montoPagado: this.montoPagado,
      metodoPago: this.metodoPago,
      comprobante: '',
      tipoTransaccion: this.tipoTransaccion
    };

    // 👇 ESTA ES LA PARTE QUE SE REEMPLAZÓ 👇
    if (this.selectedFiles.length > 0) {
      const uploads = this.selectedFiles.map(file =>
        this.ventaService.uploadComprobante(file).toPromise()
      );

      Promise.all(uploads).then((results:any) => {
        const paths = results.map((r:any) => r.path);
        nuevoEnviarServicio.comprobante = paths.join(',');
        this.enviarVenta(nuevoEnviarServicio);
      }).catch((err) => {
        console.error("Error al subir imágenes:", err);
        Swal.fire('❌ Error', 'No se pudieron subir los comprobantes', 'error');
      });
    } else {
      this.enviarVenta(nuevoEnviarServicio);
    }
  }
  // En el component.ts, agrega esta función
private cargarImagenComoBase64(url: string): Promise<string> {
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.crossOrigin = 'Anonymous';
    img.onload = () => {
      const canvas = document.createElement('canvas');
      canvas.width = img.width;
      canvas.height = img.height;
      const ctx = canvas.getContext('2d');
      ctx!.drawImage(img, 0, 0);
      resolve(canvas.toDataURL('image/png'));
    };
    img.onerror = reject;
    img.src = url;
  });
}
/*generarPDFComprobante() {

  const doc = new jsPDF();

  doc.setFontSize(14);
  doc.text('COMPROBANTE DE VENTA', 105, 15, { align: 'center' });

  doc.setFontSize(10);
  doc.text(`Cliente: ${this.clientes.find(c => c.idCliente === this.idClienteSeleccionado)?.nombreCliente}`, 14, 25);
  doc.text(`Fecha: ${new Date(this.fechaventa!).toLocaleDateString()}`, 14, 32);
  doc.text(`Vendedor: ${this.nombreVendedor}`, 14, 39);
  doc.text(`Método de Pago: ${this.metodoPago}`, 14, 46);

  const body = this.VentaList.map(item => ([
    item.SubProducto,
    item.cantidad,
    `S/. ${item.precioUnitario.toFixed(2)}`,
    `S/. ${(item.cantidad * item.precioUnitario).toFixed(2)}`
  ]));

  autoTable(doc, {
    startY: 55,
    head: [['Producto', 'Cantidad', 'Precio Unit.', 'Subtotal']],
    body
  });

  doc.text(`TOTAL: S/. ${this.totalVenta.toFixed(2)}`, 14, (doc as any).lastAutoTable.finalY + 10);

  doc.save(`comprobante_${Date.now()}.pdf`);
}*/
async  generarPDFComprobante(respBackend?: any, tipo: 'BOLETA' | 'NORMAL' = 'NORMAL') {
  const doc = new jsPDF();
  const data = respBackend || {};
console.log('respBackend', respBackend);
console.log('VentaList', this.VentaList);
console.log('totalVenta', this.totalVenta);
console.log('metodoPago', this.metodoPago);
console.log('idClienteSeleccionado', this.idClienteSeleccionado);
console.log('clientes', this.clientes);

  if (tipo === 'BOLETA') {
    const serie   = data.serie  || 'B001';
    const numero  = data.numero || '00000000';
    const hash    = data.hash   || '';
    const idVenta = data.idVenta || 0;

    const empresaNombre    = data.empresaNombre    || '';
    const empresaRuc       = data.empresaRuc       || '';
    const empresaDireccion = data.empresaDireccion || '';
    const empresaEmail     = data.empresaEmail     || 'admin@gmail.com';

    try {
      const logoBase64 = await this.cargarImagenComoBase64('/img/logo.png');
     doc.addImage(logoBase64, 'PNG', 14, 10, 28, 18);
    } catch {
      console.warn('No se pudo cargar el logo');
    }
    if (idVenta) {
        try {
        const qrBase64 = await this.cargarImagenComoBase64(
          `${this.apiHost}/QR/${idVenta}.jpg`
        );
        doc.addImage(qrBase64, 'JPEG', 130, 180, 50, 50);
        doc.setFontSize(7);
        doc.setFont('helvetica', 'normal');
        doc.text(`Código Hash: ${hash}`, 130, 234);
      } catch {
        console.warn('No se pudo cargar el QR');
        // Si no hay QR, solo poner el hash
        if (hash) {
          doc.setFontSize(7);
          doc.text(`Código Hash: ${hash}`, 14, 234);
        }
      }
    }
    // CABECERA EMPRESA (izquierda)
    doc.setFontSize(12);
    doc.setFont('helvetica', 'bold');
    doc.text(empresaNombre, 46, 16);
    //doc.text('CORPORACION TESMA S.A.C.', 46, 16);
    doc.setFontSize(9);
    doc.setFont('helvetica', 'normal');
    doc.text(`RUC ${empresaRuc}`, 46, 21);
    //doc.text('RUC 20613314319', 46, 21);
    doc.setFontSize(7.5);
    doc.text(empresaDireccion, 46, 25);
    doc.text(`Email: ${empresaEmail}`, 46, 29);
    //doc.text('AV. SANTA ANA, MZ. A32, LTE. 39, SANTA ANITA, LIMA - LIMA', 46, 25);
    //doc.text('Email: admin@gmail.com', 46, 29);

    // CUADRO BOLETA (derecha)
    doc.setDrawColor(0);
    doc.rect(135, 10, 61, 23);
    doc.setFontSize(9);
    doc.setFont('helvetica', 'bold');
    doc.text('BOLETA DE VENTA', 165.5, 16, { align: 'center' });
    doc.text('ELECTRÓNICA', 165.5, 20, { align: 'center' });
    doc.setFontSize(12);
    doc.text(`${serie}-${numero.padStart(8, '0')}`, 165.5, 28, { align: 'center' });

    // LÍNEA SEPARADORA
    doc.line(14, 40, 196, 40);

    // DATOS DEL CLIENTE
const clienteEncontrado = this.clientes.find(
  c => Number(c.idCliente) === Number(this.idClienteSeleccionado)
);

console.log('clienteEncontrado', clienteEncontrado);

const clienteNombre =
  clienteEncontrado?.nombreCliente ||
  this.VentaList[0]?.cliente ||
  '';

    doc.setFontSize(9);
    doc.setFont('helvetica', 'normal');
    doc.text(`FECHA DE EMISIÓN  : ${new Date(this.fechaventa!).toLocaleDateString()}`, 14, 52);
    doc.text(`FECHA DE VENCIMIENTO  : ${new Date(this.fechaventa!).toLocaleDateString()}`, 14, 58);
    console.log('clienteNombre', clienteNombre);
    doc.text(`CLIENTE: ${clienteNombre}`, 14, 64);
    doc.text(`VENDEDOR: ${this.nombreVendedor}`, 14, 70);

    // TABLA DE PRODUCTOS
    const body = this.VentaList.map(item => ([
      item.idSubProducto?.toString() || '',
      item.cantidad,
      'NIU',
      item.SubProducto,
      `${item.precioUnitario.toFixed(2)}`,
      '0',
      `${(item.cantidad * item.precioUnitario).toFixed(2)}`
    ]));
console.log('body', body);
    autoTable(doc, {
      startY: 78,
      head: [['COD.', 'CANT.', 'UNIDAD', 'DESCRIPCIÓN', 'P.UNIT', 'DTO.', 'TOTAL']],
      body: body,
      styles: { fontSize: 8 },
      headStyles: { fillColor: [41, 128, 185] }
    });

    const finalY = (doc as any).lastAutoTable.finalY;

    // TOTALES
    const subtotal = this.totalVenta / 1.18;
    const igv      = this.totalVenta - subtotal;

    doc.setFontSize(9);
    doc.text(`OP. GRAVADAS: S/ ${subtotal.toFixed(2)}`,  196, finalY + 8,  { align: 'right' });
    doc.text(`IGV: S/ ${igv.toFixed(2)}`,                196, finalY + 14, { align: 'right' });
    doc.setFont('helvetica', 'bold');
    doc.text(`TOTAL A PAGAR: S/ ${this.totalVenta.toFixed(2)}`, 196, finalY + 20, { align: 'right' });

    // CONDICIÓN DE PAGO
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(9);
    doc.text(`CONDICIÓN DE PAGO: Contado`, 14, finalY + 30);
    doc.setFont('helvetica', 'normal');
    doc.text(`PAGOS:`, 14, finalY + 36);
    doc.text(`• ${this.metodoPago} - S/ ${this.totalVenta.toFixed(2)}`, 14, finalY + 42);

    // VENDEDOR
    doc.text(`Vendedor:`, 14, finalY + 52);
    doc.setFont('helvetica', 'bold');
    doc.text(this.nombreVendedor, 14, finalY + 58);

    // QR (si existe en wwwroot/QR/)
    // if (hash) {
    //   doc.setFontSize(8);
    //   doc.setFont('helvetica', 'normal');
    //   doc.text(`Código Hash: ${hash}`, 130, finalY + 58);
    // }

    doc.save(`Boleta_${serie}-${numero}.pdf`);

  } else {
    // COMPROBANTE NORMAL (sin TESMA) - tu diseño actual
    const idVenta = data.idVenta || '0000';
    doc.setFontSize(14);
    doc.text('COMPROBANTE DE VENTA', 105, 15, { align: 'center' });
    doc.setFontSize(10);
    doc.text(`Control Interno N°: ${idVenta}`, 105, 21, { align: 'center' });

const clienteEncontrado = this.clientes.find(
  c => Number(c.idCliente) === Number(this.idClienteSeleccionado)
);

console.log('clienteEncontrado', clienteEncontrado);

const clienteNombre =
  clienteEncontrado?.nombreCliente ||
  this.VentaList[0]?.cliente ||
  '';

    doc.text(`Cliente: ${clienteNombre}`, 14, 30);
    doc.text(`Fecha: ${new Date(this.fechaventa!).toLocaleDateString()}`, 14, 37);
    doc.text(`Vendedor: ${this.nombreVendedor}`, 14, 44);
    doc.text(`Método de Pago: ${this.metodoPago}`, 14, 51);

    const body = this.VentaList.map(item => ([
      item.SubProducto,
      item.cantidad,
      `S/. ${item.precioUnitario.toFixed(2)}`,
      `S/. ${(item.cantidad * item.precioUnitario).toFixed(2)}`
    ]));

    autoTable(doc, {
      startY: 60,
      head: [['Producto', 'Cantidad', 'Precio Unit.', 'Subtotal']],
      body: body
    });

    const finalY = (doc as any).lastAutoTable.finalY + 10;
    doc.text(`TOTAL: S/. ${this.totalVenta.toFixed(2)}`, 14, finalY);
    doc.save(`Comprobante_${idVenta}.pdf`);
  }
}
guardarClienteRapido() {

  if (!this.nuevoCliente.dni || !this.nuevoCliente.nombreCliente) {
    Swal.fire('Atención', 'DNI y Nombre son obligatorios', 'warning');
    return;
  }

  const existe = this.clientes.find(c =>
    c.dni === this.nuevoCliente.dni
  );

  if (existe) {
    Swal.fire('Atención', 'Este cliente ya existe', 'warning');
    return;
  }

  this.ClienteService.AddCliente(this.nuevoCliente).subscribe({

    next: (resp: any) => {

      Swal.fire('✅ Cliente registrado', '', 'success');

      // 🔴 GUARDAMOS EL DNI ANTES DE LIMPIAR
      const dniGuardado = this.nuevoCliente.dni;

      this.cerrarModalCliente();

      // 🔥 RECARGAMOS CLIENTES
      this.ClienteService.getCliente().subscribe({
        next: (clientesActualizados) => {

          this.clientes = [...clientesActualizados]; // 🔥 IMPORTANTE (nueva referencia)

          const clienteNuevo = this.clientes.find(
            c => c.dni === dniGuardado
          );

          if (clienteNuevo) {

            // 🔥 RESET primero
            this.idClienteSeleccionado = null;

            // 🔥 FORZAR DETECCIÓN
            setTimeout(() => {
              this.idClienteSeleccionado = clienteNuevo.idCliente;
            });

          }

        }
      });

    },

    error: () => {
      Swal.fire('Error', 'No se pudo registrar', 'error');
    }

  });

}
buscarDniRapido() {

  if (!this.nuevoCliente.dni) return;
  const existe = this.clientes.find(c => c.dni === this.nuevoCliente.dni);
   if (existe) {
   Swal.fire('Atención', 'Este cliente ya existe', 'warning');

    // opcional: autocompletar
    this.nuevoCliente.nombreCliente = existe.nombreCliente;
    this.nuevoCliente.telefono = existe.telefono;

    return;
  }

  const dni = this.nuevoCliente.dni;

  if (!dni || dni.length !== 8) return;

  this.ClienteService.buscarDni(dni).subscribe({

    next: (resp) => {

      let nombre = '';

      if (resp.nombre) {
        nombre = resp.nombre;
      } else if (resp.data?.nombre_completo) {
        nombre = resp.data.nombre_completo;
      }

      if (nombre) {
        this.nuevoCliente.nombreCliente = nombre;
      } else {
        this.nuevoCliente.nombreCliente = 'CLIENTE ' + dni;
      }

    },

    error: () => {
      this.nuevoCliente.nombreCliente = 'CLIENTE ' + dni;
    }

  });

}
abrirNuevoCliente() {

  this.nuevoCliente = {
    nombreCliente: '',
    dni: '',
    telefono: '',
     direccion: ''
  };

  this.mostrarModalCliente = true;

}
cerrarModalCliente() {
  this.mostrarModalCliente = false;

  this.nuevoCliente = {
    nombreCliente: '',
    dni: '',
    telefono: '',
     direccion: ''
  };
}

/*  private enviarVenta(ventaData: VentaEnviar) {
    this.ventaService.registrarVenta(ventaData).subscribe({
      next: (resp) => {
        Swal.fire({
          icon: 'success',
          title: 'Venta registrada correctamente',
          timer: 1500,
          showConfirmButton: false
        });

        // 🔹 Limpia todo el formulario
        this.resetFormularioVenta();
        this.cargarCombos();
      },
      error: (err) => {
        console.error(err);
        Swal.fire('❌ Error', 'No se pudo registrar la venta', 'error');
      }
    });
  } */
private enviarVenta(ventaData: VentaEnviar) {
  this.ventaService.registrarVenta(ventaData).subscribe({
    next:async  (resp: any) => {

      Swal.fire({
        icon: 'success',
        title: resp.mensaje || 'Venta registrada correctamente',
        text: `N° de Venta: ${resp.idVenta}`,
        timer: 2000,
        showConfirmButton: true
      });

      // 🔹 VALIDACIÓN DEL TIPO DE COMPROBANTE
      if (this.tipoTransaccion === 'COMPLETA') {
        try {
          if (resp.esElectronico) {
            // Opción A: Es TESMA -> Enviamos los datos para la Boleta Electrónica
            await  this.generarPDFComprobante(resp, 'BOLETA');
          } else {
            // Opción B: NO es TESMA -> Generamos un ticket interno normal
            await  this.generarPDFComprobante(resp, 'NORMAL');
          }
        } catch (pdfError) {
          console.error("Error al generar el PDF:", pdfError);
        }
      }

      this.resetFormularioVenta();
      this.cargarCombos();
    },
    error: (err) => {
      console.error('Error:', err);
      Swal.fire('❌ Error', 'Hubo un problema en el servidor', 'error');
    }
  });
}
}
