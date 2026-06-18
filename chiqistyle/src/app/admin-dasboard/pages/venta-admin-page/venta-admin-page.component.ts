import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgSelectModule } from '@ng-select/ng-select';
import { Cliente } from '@products/interfaces/cliente.interface';
import { ProductoDisponibles } from '@products/interfaces/compra.interface';
import { ProductoVentaInfo as MConsultaProductoVenta, listprodMostrar, VentaEnviar, VentaProductoEnviar } from '@products/interfaces/venta.interface';
import { ClienteService } from '@products/services/cliente.service';
import { CompraService } from '@products/services/compra.service';
import { VentaService } from '@products/services/venta.service';
import Swal from 'sweetalert2';
import jsPDF from 'jspdf';
import autoTable from 'jspdf-autotable';


@Component({
  selector: 'app-venta-admin-page',
  standalone: true,
  imports: [CommonModule,FormsModule,NgSelectModule],
  templateUrl: './venta-admin-page.component.html',
  styleUrls: ['./venta-admin-page.component.scss']
})
export class VentaAdminPageComponent implements OnInit{
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
  agregarVenta(){
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
    }

    if (this.selectedFiles.length > 0) {
      // 🔹 1. Subir comprobante primero
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
      // La validación anterior previene este camino, pero se mantiene la estructura.
      this.enviarVenta(nuevoEnviarServicio);
    }

 if (
    this.tipoTransaccion === 'COMPLETA' &&
    this.metodoPago !== 'YAPE TESMA' &&  this.metodoPago !== 'TRANSFERENCIA BCP TESMA'  &&  this.metodoPago !== 'PLIN TESMA' &&  this.metodoPago !== 'TRANSFERENCIA BBVA TESMA'
  ) {
    this.generarPDFComprobante();
  }

  }

generarPDFComprobante() {

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

  private enviarVenta(ventaData: VentaEnviar) {
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
  }
}
