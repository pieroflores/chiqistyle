import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { productoService } from '@products/services/productoPrincipal.service';
import { Categoria } from '@products/interfaces/categoria.interface';
import { ProductoPrincipal } from '@products/interfaces/productoPrincipal';
import { Color } from '@products/interfaces/color.interface';
import { SubProducto } from '@products/interfaces/subProducto.interface';
import { Talla } from '@products/interfaces/talla.interface';
import Swal from 'sweetalert2';
import { NgSelectModule } from '@ng-select/ng-select';
import { ColorService } from '@products/services/color.service';
import { ProveedorService } from '@products/services/proveedor.service';
import { Proveedor } from '@products/interfaces/proveedor.interface';

@Component({
  selector: 'app-products-admin-page',
  standalone: true,
  imports: [CommonModule, FormsModule,NgSelectModule],
  templateUrl: './products-admin-page.component.html',
  styleUrls: ['./products-admin-page.component.scss']
})
export class ProductsAdminPageComponent implements OnInit{
  mostrarTallas = false;
  categorias: Categoria[] = [];
  color: Color[] = [];
  talla: Talla[] = [];
  productoPrincipal: ProductoPrincipal[] = [];
  filtrarproducto: ProductoPrincipal[]=[];
  visualizarSubProducto: boolean;
  productoSeleccionado: ProductoPrincipal | null = null;
  subProductos: SubProducto[] = [];
  producto: any = {
    nombreProducto: '',
    idCategoria: null,
    fotoProducto: null,
    idProveedor: null
  };
  subProducto: any = {
  idSubProducto: null,
   idColores: [] as number[],
  idProductoPrincipal: null,
  idTallas: [] as number[], // varias tallas seleccionadas
  precioCompra: '',
  precioVenta: '',
  precioVentaPorMayor: '',
  precioVentaLiquidacion: ''
  }
mostrarColores = false;
  selectedFile: File | null = null;
  previewUrl: string | ArrayBuffer | null = null;
  mostrarModalColor = false;

nuevoColor:any = {
  nombreColor:'',
  abreviatura:''
};
proveedores : Proveedor[] = [];
idProveedorSeleccionado: number | null = null;
proveedorService= inject(ProveedorService)
textoBuscar: string = ''; // <-- Esta variable debe llamarse igual que en el HTML


  constructor(private productoService: productoService, private colorService: ColorService) {}

  // onFileSelected(event: Event): void {
  //   const input = event.target as HTMLInputElement;
  //   if (input.files && input.files.length > 0) {
  //     this.selectedFile = input.files[0];
  //     const reader = new FileReader();
  //     reader.onload = () => {
  //       this.previewUrl = reader.result;
  //     };
  //     reader.readAsDataURL(this.selectedFile);
  //   }
  // }
  onFileSelected(event: Event): void {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files.length > 0) {
    this.selectedFile = input.files[0];
    const reader = new FileReader();
    reader.onload = () => {
      this.previewUrl = reader.result;
    };
    reader.readAsDataURL(this.selectedFile);
  }
}
get filtrarproductos() {
  return this.productoPrincipal.filter(p =>
    p.nombreProducto.toLowerCase().includes(this.textoBuscar.toLowerCase())
  );
}
toggleColorSeleccionado(idColor: number) {
  const index = this.subProducto.idColores.indexOf(idColor);
  if (index > -1) {
    this.subProducto.idColores.splice(index, 1);
  } else {
    this.subProducto.idColores.push(idColor);
  }
}

obtenerTextoColoresSeleccionados(): string {
  if (!this.subProducto.idColores.length) return 'Selecciona colores';

  if (this.subProducto.idColores.length === 1) {
    const c = this.color.find(x => x.idColor === this.subProducto.idColores[0]);
    return c ? c.nombreColor : '';
  }

  return `${this.subProducto.idColores.length} colores seleccionados`;
}
onPasteImage(event: ClipboardEvent): void {
  const items = event.clipboardData?.items;
  if (!items) return;

  for (const item of items) {
    if (item.type.indexOf('image') !== -1) {
      const file = item.getAsFile();
      if (file) {
        this.selectedFile = file;
        const reader = new FileReader();
        reader.onload = () => {
          this.previewUrl = reader.result;
        };
        reader.readAsDataURL(file);
      }
    }
  }
}
eliminarImagen(event: Event): void {
  event.stopPropagation(); // evita abrir el fileInput al hacer clic
  this.selectedFile = null;
  this.previewUrl = null;
}
  ngOnInit(): void {
this.cargarCategoria();
this.cargarProductos();
this.cargarColor();
this.cargarTalla();
this.cargarProveedor();
this.visualizarSubProducto= false;
 }
 cerrarModalSubProducto() {
  this.visualizarSubProducto = false;
  this.subProductos = []; // Limpiamos el arreglo para que no queden datos cacheados
  this.productoSeleccionado = null; // <-- AGREGAR ESTA LÍNEA
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
toggleTallaSeleccionada(idTalla: number) {
  const index = this.subProducto.idTallas.indexOf(idTalla);
  if (index > -1) {
    this.subProducto.idTallas.splice(index, 1);
  } else {
    this.subProducto.idTallas.push(idTalla);
  }
}
obtenerTextoTallasSeleccionadas(): string {
  if (!this.subProducto.idTallas || this.subProducto.idTallas.length === 0) {
    return 'Selecciona tallas';
  }
  if (this.subProducto.idTallas.length === 1) {
    const talla = this.talla.find(t => t.idTalla === this.subProducto.idTallas[0]);
    return talla ? talla.nombreTalla : '';
  }
  return `${this.subProducto.idTallas.length} tallas seleccionadas`;
}
  cargarCategoria() {
  this.productoService.getCategoria().subscribe({
    next: (resp) => {
      this.categorias = resp;

    },
    error: (err) => {
      console.error('Error al cargar Categoria:', err);
    }
  });
}

 cargarTalla() {
  this.productoService.getTalla().subscribe({
    next: (resp) => {
      this.talla = resp;

    },
    error: (err) => {
      console.error('Error al cargar tallas:', err);
    }
  });
}

cargarColor(){
  this.productoService.getColor().subscribe({
    next: (resp) => {
      this.color = resp;

    },
    error: (err) => {
      console.error('Error al cargar Color:', err);
    }
  });
}

cargarProductos() {
  this.productoService.getProducto().subscribe({
    next: (resp) => {
      this.productoPrincipal = resp;
      this.filtrarproducto= resp;

    },
    error: (err) => {
      console.error('Error al cargar Producto:', err);
    }
  });
}
verSubProducto(prod: ProductoPrincipal) {
  this.visualizarSubProducto = true;
  this.productoSeleccionado = prod; // <-- AGREGAR ESTA LÍNEA

  this.productoService.getSubProductosPorProducto(prod.idProductoPrincipal).subscribe({
    next: (resp) => {
      console.log(resp)
      this.subProductos = resp;
      console.log("✅ Subproductos cargados:", resp);
    },
    error: (err) => {
      console.error("❌ Error al cargar subproductos:", err);
    }
  });
}

onSubmitSub(formSub: any){
this.subProducto.precioCompra = Number(this.subProducto.precioCompra);
this.subProducto.precioVenta = Number(this.subProducto.precioVenta);
 if (!this.subProducto.idTallas || this.subProducto.idTallas.length === 0) {
    Swal.fire({
      icon: 'warning',
      title: 'Atención',
      text: 'Debes seleccionar al menos una talla',
    });
    return;
  }
     const registros = [];

this.subProducto.idColores.forEach((idColor: number) => {
  this.subProducto.idTallas.forEach((idTalla: number) => {
    registros.push({
      idProductoPrincipal: this.subProducto.idProductoPrincipal,
      idColor,
      idTalla,
      precioCompra: this.subProducto.precioCompra,
      precioVenta: this.subProducto.precioVenta,
      precioVentaPorMayor: this.subProducto.precioVentaPorMayor,
      precioVentaLiquidacion: this.subProducto.precioVentaLiquidacion
    });
  });
});
   let exitosos = 0;
  let fallidos = 0;

  if (!this.subProducto.idColores.length) {
  Swal.fire('Atención', 'Debes seleccionar al menos un color', 'warning');
  return;
}

if (!this.subProducto.idTallas.length) {
  Swal.fire('Atención', 'Debes seleccionar al menos una talla', 'warning');
  return;
}

  registros.forEach((sub) => {
    this.productoService.addSubProducto(sub).subscribe({
      next: () => {
        exitosos++;
        if (exitosos + fallidos === registros.length) {
          Swal.fire({
            icon: 'success',
            title: 'Registro completado',
            text: `${exitosos} subproductos registrados correctamente.`,
          });
          formSub.reset();
          this.subProducto.idTallas = [];
          this.subProducto.idColores = [];
        }
      },
      error: (err) => {
        fallidos++;
        console.error('❌ Error registrando talla:', err);
      },
    });
  });

}


onSubmit(form: any) {
  if (this.selectedFile) {
    this.productoService.uploadImage(this.selectedFile).subscribe({
      next: (res) => {
        this.producto.fotoProducto = res.path;
        console.log(this.producto)
        this.productoService.addProducto(this.producto).subscribe({
          next: (resp) => {
            this.cargarProductos();
            // 🔹 Reset form y variables
            form.reset();
            this.previewUrl = null;
            this.selectedFile = null;

            Swal.fire({
              icon: 'success',
              title: 'Producto registrado',
              text: 'El producto con imagen fue guardado correctamente',
              confirmButtonColor: '#3085d6'
            });
          },
          error: (err) => {
            Swal.fire({
              icon: 'error',
              title: 'Error',
              text: 'No se pudo guardar el producto',
              confirmButtonColor: '#d33'
            });
          }
        });
      },
      error: (err) => {
        Swal.fire({
          icon: 'error',
          title: 'Error al subir imagen',
          text: 'No se pudo subir la imagen del producto',
          confirmButtonColor: '#d33'
        });
      }
    });
  } else {
    this.productoService.addProducto(this.producto).subscribe({
      next: (resp) => {
        // 🔹 Reset form y variables
        form.reset();
        this.previewUrl = null;
        this.selectedFile = null;

        Swal.fire({
          icon: 'success',
          title: 'Producto registrado',
          text: 'El producto fue guardado correctamente (sin imagen)',
          confirmButtonColor: '#3085d6'
        });
      },
      error: (err) => {
        Swal.fire({
          icon: 'error',
          title: 'Error',
          text: 'No se pudo guardar el producto',
          confirmButtonColor: '#d33'
        });
      }
    });
  }
}
abrirModalColor(){
  this.mostrarModalColor = true;
}

cerrarModalColor(){
  this.mostrarModalColor = false;

  this.nuevoColor = {
    nombreColor:'',
    abreviatura:''
  };
}
actualizarAbreviatura(){
  if(!this.nuevoColor.nombreColor) return;

  this.nuevoColor.abreviatura = this.generarAbreviatura(this.nuevoColor.nombreColor);
}
generarAbreviatura(texto: string): string {

  const palabras = texto
    .trim()
    .toUpperCase()
    .split(' ')
    .filter(p => p.length > 0);

  if (palabras.length === 1) {
    return palabras[0].substring(0, 3);
  }

  if (palabras.length === 2) {
    const p1 = palabras[0].substring(0,3);
    const p2 = palabras[1].substring(0,4);
    return `${p1}-${p2}`;
  }

  // 3 o más palabras
  return palabras
    .map(p => p.substring(0,2))
    .join('-');

}
guardarNuevoColor(){

  if(!this.nuevoColor.nombreColor){
    Swal.fire('Atención','Ingrese el nombre del color','warning');
    return;
  }

  this.colorService.registrarColor(this.nuevoColor).subscribe({

    next:(resp:any)=>{

      Swal.fire({
        icon:'success',
        title:'Color registrado'
      });

      this.cerrarModalColor();

      // recargar colores
      this.cargarColor();

      // seleccionar automáticamente
      setTimeout(()=>{

        const colorNuevo = this.color.find(
          c=>c.nombreColor === this.nuevoColor.nombreColor
        );

        if(colorNuevo){
          this.subProducto.idColores.push(colorNuevo.idColor);
        }

      },300);

    },

    error:(err)=>{
      console.error(err);
      Swal.fire('Error','No se pudo registrar color','error');
    }

  });

}

}
