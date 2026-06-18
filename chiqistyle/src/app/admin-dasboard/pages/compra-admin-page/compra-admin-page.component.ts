import { Component, inject, OnInit } from '@angular/core';
import { ProveedorService } from '../../../products/services/proveedor.service';
import { Proveedor } from '@products/interfaces/proveedor.interface';
import { FormsModule } from '@angular/forms';
import { CommonModule,NgFor  } from '@angular/common';
import { CompraService } from '@products/services/compra.service';
import { AlmacenCombo, CompraEnviar, CompraProductoEnviar, SubProductoCombo, TipoDocumento } from '@products/interfaces/compra.interface';
import { CompraProductoList } from '../../../products/interfaces/compra.interface';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-compra-admin-page',
  imports: [CommonModule,FormsModule],
  templateUrl: './compra-admin-page.component.html',
})
export class CompraAdminPageComponent implements OnInit{
proveedores : Proveedor[] = [];
 idProveedorSeleccionado: number | null = null;
proveedorService= inject(ProveedorService)
compraService = inject(CompraService)
CompraList : CompraProductoList[] = [];
CompraProductoE: CompraProductoEnviar[] = [];
fechaCompra: Date | null = null;
 observacion: string = '';
 totalCompra: number = 0;
 cantidad: number = 0;
  precioUnitario: number = 0;
 productos: SubProductoCombo[] = [];
    ubicaciones: AlmacenCombo[] = [];
    tiposDocumento: TipoDocumento[] = [];

    // Variables para el valor seleccionado del detalle
   idSubProductoSeleccionado: number | null = null;
    idAlmacenSeleccionado: number | null = null;
    idTipoDocumentoSeleccionado: number | null = null;

 nombre: string = '';
 idUsuario: number | null = null;

 productosPrincipales: any[] = [];
tallasProducto: any[] = [];
idProductoPrincipalSeleccionado: number | null = null;
mostrarModalCantidades = false;
cantidadGeneral = 1;
usarCantidadGeneral = true;

ngOnInit(): void {
   const userData = localStorage.getItem('usuario');
      if (userData) {
    const usuario = JSON.parse(userData);
    this.nombre = usuario.nombreCompleto; // 👈 tomamos el nombre del login
    this.idUsuario = usuario.idUsuario;
      }

  this.CompraList=[];
this.cargarProveedor();
this.cargarCombos();


}
cargarProveedor(){
  this.proveedorService.getProveedor().subscribe({
    next:(resp) => {
      this.proveedores= resp;

    } ,
    error: (err) => {
      //console.error('Error al cargar Proveedor:', err);
    }
  })
}
abrirModalCantidades() {
  if (!this.idProductoPrincipalSeleccionado) return;

  this.compraService
    .obtenerSubProductosPorProducto(this.idProductoPrincipalSeleccionado)
    .subscribe({
      next: resp => {
        this.tallasProducto = resp.map(x => ({
          ...x,
          cantidad: 1
        }));
        if (resp.length > 0) {
    this.precioUnitario = resp[0].precioCompra;
  }
        this.mostrarModalCantidades = true;
      }
    });
}
confirmarDistribucion() {

  if (this.usarCantidadGeneral) {
    this.tallasProducto = this.tallasProducto.map(t => ({
      ...t,
      cantidad: this.cantidadGeneral
    }));
  }

  this.mostrarModalCantidades = false;
}
cerrarModal() {
  this.mostrarModalCantidades = false;

  // 🔥 fuerza el cierre visual del select
  setTimeout(() => {
    (document.activeElement as HTMLElement)?.blur();
  }, 0);

  this.tallasProducto = [];
  this.cantidadGeneral = 1;
  this.usarCantidadGeneral = true;
  this.precioUnitario=0;
}
cargarTallasProducto() {
  if (!this.idProductoPrincipalSeleccionado) return;

  this.compraService
      .obtenerSubProductosPorProducto(this.idProductoPrincipalSeleccionado)
      .subscribe({
        next: resp => {
          this.tallasProducto = resp;
          console.log(this.tallasProducto)
        },
        error: err => {
          console.error('Error cargando tallas', err);
        }
      });
}

cargarCombos(){
        this.compraService.cargarCombos().subscribe({
            next: (resp) => {
              console.log(resp)
                this.productos = resp.productos;

                 //this.productosPrincipales = resp.productoPrincipal; // 👈 ESTA LÍNEA
                 console.log(this.productosPrincipales)
                this.ubicaciones = resp.ubicaciones;
                this.tiposDocumento = resp.tiposDocumento;
            },
            error: (err) => {
                //console.error('Error al cargar Combos:', err);
            }
        });
    }
   onProveedorChange() {

  // 🔥 limpiar todo primero
  this.productosPrincipales = [];
  this.idProductoPrincipalSeleccionado = null;
  this.tallasProducto = [];

  if (!this.idProveedorSeleccionado) {
    return;
  }

  this.compraService
    .cargarProductosPorProveedor(this.idProveedorSeleccionado)
    .subscribe({
      next: (resp) => {

        // 🔥 si no hay productos
        if (!resp || resp.length === 0) {

          Swal.fire({
            icon: 'info',
            title: 'Sin productos',
            text: 'Este proveedor no tiene productos nuevos para agregar.'
          });

          return;
        }

        this.productosPrincipales = resp;
      },
      error: (err) => {
        console.error(err);
      }
    });
}
cargarProductosPorProveedor(idProveedor: number) {

  this.compraService
    .cargarProductosPorProveedor(idProveedor)
    .subscribe({
      next: (resp) => {

        this.productosPrincipales = resp;

        console.log("Productos filtrados:", resp);

      },
      error: (err) => {
        console.error(err);
      }
    });

}

calcularTotal() {
  this.totalCompra = this.CompraList.reduce((acc, item) => {
    return acc + (item.cantidad * item.precioUnitario);
  }, 0);
}
// agregarDetalle(){
//   if (!this.idSubProductoSeleccionado || !this.cantidad || !this.precioUnitario || !this.idAlmacenSeleccionado  || !this.idProveedorSeleccionado  || !this.fechaCompra  || !this.idTipoDocumentoSeleccionado) {
//       Swal.fire({
//       icon: 'warning',
//       title: 'Campos incompletos',
//       text: 'Completa todos los campos del producto antes de agregarlo.',
//       confirmButtonColor: '#3085d6'
//     });
//     return;
//     }

//     const subProd = this.productos.find(p => p.idSubProducto === this.idSubProductoSeleccionado);
// const ubi = this.ubicaciones.find(u => u.idAlmacen === this.idAlmacenSeleccionado);
// const tipoDoc = this.tiposDocumento.find(t => t.idTipoDocumentoMercaderia === this.idTipoDocumentoSeleccionado);
// const prov = this.proveedores.find(p => p.idProveedor === this.idProveedorSeleccionado);

//     const nuevo: CompraProductoEnviar = {
//       idSubProducto: this.idSubProductoSeleccionado,
//       cantidad: this.cantidad,
//       precioUnitario: this.precioUnitario,
//       idAlmacen: this.idAlmacenSeleccionado,
//     };

//     this.CompraProductoE.push(nuevo);



// const nuevoList: CompraProductoList = {
//   Proveedor: prov.nombreProveedor,
//   fechaCompra: this.fechaCompra,
//   TipoDocumento: tipoDoc ? tipoDoc.nombre : "",
//   observacion: this.observacion,
//   SubProducto: subProd ? subProd.productoVariable : "—",
//   cantidad: this.cantidad,
//   precioUnitario: this.precioUnitario,
//   Almacen: ubi ? ubi.ubicacion : "—"
// };
// this.CompraList.push(nuevoList);
// this.calcularTotal();

//  Swal.fire({
//     icon: 'success',
//     title: 'Producto agregado',
//     text: 'El producto fue añadido correctamente al detalle.',
//     showConfirmButton: false,
//     timer: 1500
//   });

// }
reabrirModal() {
  if (this.idProductoPrincipalSeleccionado) {
    this.abrirModalCantidades();
  }
}

agregarDetalle() {
this.cantidadGeneral = 0
  if (!this.idProductoPrincipalSeleccionado ||

      !this.precioUnitario ||
      !this.idAlmacenSeleccionado ||
      !this.idProveedorSeleccionado ||
      !this.fechaCompra ||
      !this.idTipoDocumentoSeleccionado) {

    Swal.fire({
      icon: 'warning',
      title: 'Campos incompletos',
      text: 'Completa todos los campos antes de agregar.'
    });
    return;
  }

  if (!this.tallasProducto.length) {
    Swal.fire({
      icon: 'error',
      title: 'Producto sin tallas',
      text: 'Este producto no tiene tallas registradas.'
    });
    return;
  }

  this.tallasProducto.forEach(t => {

    const nuevo: CompraProductoEnviar = {
      idSubProducto: t.idSubProducto,
      cantidad: t.cantidad,
      precioUnitario: this.precioUnitario,
      idAlmacen: this.idAlmacenSeleccionado,
    };

    this.CompraProductoE.push(nuevo);

    const nuevoList: CompraProductoList = {
      Proveedor: this.proveedores.find(p => p.idProveedor === this.idProveedorSeleccionado)?.nombreProveedor || '',
      fechaCompra: this.fechaCompra!,
      TipoDocumento: this.tiposDocumento.find(t => t.idTipoDocumentoMercaderia === this.idTipoDocumentoSeleccionado)?.nombre || '',
      SubProducto: t.descripcionSubproducto,
       cantidad: t.cantidad,
      precioUnitario: this.precioUnitario,
      Almacen: this.ubicaciones.find(u => u.idAlmacen === this.idAlmacenSeleccionado)?.ubicacion || ''
    };

    this.CompraList.push(nuevoList);

  });

  this.calcularTotal();

  Swal.fire({
    icon: 'success',
    title: 'Producto agregado',
    text: `Se registraron ${this.tallasProducto.length} tallas automáticamente.`,
    timer: 1500,
    showConfirmButton: false
  });

}


eliminarDetalle(index: number) {
  this.CompraList.splice(index, 1);
  this.CompraProductoE.splice(index, 1);
  this.calcularTotal();
   Swal.fire({
    icon: 'info',
    title: 'Producto eliminado',
    text: 'El producto fue quitado del detalle.',
    showConfirmButton: false,
    timer: 1200
  });
}
finalizarCompra() {
 if (this.CompraProductoE.length === 0) {
    Swal.fire({
      icon: 'error',
      title: 'Sin productos',
      text: 'Debes agregar al menos un producto antes de finalizar la compra.',
    });
    return;
  }
    //idProveedor: this.idProveedorSeleccionado,
      //fechaCompra: this.fechaCompra,
      //tipoDocumento: tipoDoc ? tipoDoc.nombre : "",
      //observacion: this.observacion,
   const compraEnviar: CompraEnviar = {
    idProveedor: this.idProveedorSeleccionado!,
    fechaCompra: this.fechaCompra!,
    idUsuario: this.idUsuario, // 🔹 aquí pondrás el usuario logueado
    documentoReferencia: (this.tiposDocumento.find(t => t.idTipoDocumentoMercaderia === this.idTipoDocumentoSeleccionado)).nombre, // 🔹 enlaza con tu input de documento
    observacion: this.observacion,
    detalle: this.CompraProductoE
  };
   this.compraService.AddCompra(compraEnviar).subscribe({
     next: (resp) => {
      Swal.fire({
         icon: 'success',
          title: 'Compra registrada',
       text: 'La compra se registró correctamente.',
          confirmButtonColor: '#28a745'
       });
        this.CompraList = [];
       this.CompraProductoE = [];
       this.totalCompra=0;
        this.resetFormulario();
        this.cargarCombos();
      },
     error: (err) => {
     console.error("❌ Error al registrar compra:", err);
       Swal.fire({
       icon: 'error',
      title: 'Error',
        text: 'Ocurrió un error al registrar la compra.',
        confirmButtonColor: '#d33'
       });
     }
   });
}
resetFormulario() {
  // Limpia las listas
  this.CompraList = [];
  this.CompraProductoE = [];
  this.totalCompra = 0;

  // Limpia campos de cabecera
  this.idProveedorSeleccionado = null;
  this.fechaCompra = null;
  this.idTipoDocumentoSeleccionado = null;
  this.observacion = '';

  // Limpia campos de detalle
  this.idSubProductoSeleccionado = null;
  this.idAlmacenSeleccionado = null;
  this.cantidad = 0;
  this.precioUnitario = 0;
}
}
