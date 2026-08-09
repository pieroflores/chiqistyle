import { Component, inject, OnInit,ElementRef, HostListener, ViewChild } from '@angular/core';
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
import { TallaSeleccionada } from '@products/interfaces/tallaSeleccionada.interface';
import { TallaService } from '@products/services/talla.service';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-products-admin-page',
  standalone: true,
  imports: [CommonModule, FormsModule,NgSelectModule],
  templateUrl: './products-admin-page.component.html',
  styleUrls: ['./products-admin-page.component.scss']
})
export class ProductsAdminPageComponent implements OnInit{
  apiHost = environment.apiUrl.replace(/\/api$/, '');
  mostrarTallas = false;
  categorias: Categoria[] = [];
  color: Color[] = [];
  talla: Talla[] = [];
  private elementRef = inject(ElementRef);
  mostrarModalTalla = false;

nuevaTalla: any = {
  nombreTalla: '',
  abreviatura: ''
};
  tallaService = inject(TallaService);
  productoPrincipal: ProductoPrincipal[] = [];
  filtrarproducto: ProductoPrincipal[]=[];
  visualizarSubProducto: boolean;
  productoSeleccionado: ProductoPrincipal | null = null;
  subProductos: SubProducto[] = [];
  mostrarDrawerEditar = false;
  editarSubProducto: any = {
    idSubProducto: 0,
    idProductoPrincipal: 0,
    idColor: null,
    idTalla: null,
    precioCompra: 0,
    precioVenta: 0,
    precioVentaPorMayor: 0,
    precioVentaLiquidacion: 0
};
//mostrarModalEditarSubProducto = false;
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
  //idTallas: [] as number[], // varias tallas seleccionadas
   idTallas:[] as TallaSeleccionada[],
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
textoBuscarColor: string = '';
@ViewChild('coloresDropdown')
coloresDropdown!: ElementRef;
textoBuscarTalla: string = '';
pasoActual: number = 1;
mostrarDrawerTalla: boolean = false;
tallaEnEdicion: any = null;
tabActivo: 'catalogo' | 'crear' = 'catalogo';
mostrarFormProducto: boolean = true;
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
@HostListener('document:click', ['$event'])
onDocumentClick(event: MouseEvent) {

  const target = event.target as HTMLElement;

  if (
    this.mostrarColores &&
    this.coloresDropdown &&
    !this.coloresDropdown.nativeElement.contains(target)
  ) {
    this.mostrarColores = false;
    this.textoBuscarColor = '';
  }

  if (
    this.mostrarTallas &&
    this.tallasDropdown &&
    !this.tallasDropdown.nativeElement.contains(target)
  ) {
    this.mostrarTallas = false;
    this.textoBuscarTalla = '';
  }
}
@ViewChild('tallasDropdown')
tallasDropdown!: ElementRef;
get filtrarproductos() {
  return this.productoPrincipal.filter(p =>
    p.nombreProducto.toLowerCase().includes(this.textoBuscar.toLowerCase())
  );
}
get tallasFiltradas(): Talla[] {

  if (!this.textoBuscarTalla.trim()) {
    return this.talla;
  }

  return this.talla.filter(t =>
    t.nombreTalla
      .toLowerCase()
      .includes(this.textoBuscarTalla.toLowerCase())
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
onCambioProducto() {
  this.subProducto.idColores = [];
  this.subProducto.idTallas = [];
}
siguientePaso() {
  if (this.pasoActual === 1 && !this.subProducto.idProductoPrincipal) return;
  if (this.pasoActual === 2 && (!this.subProducto.idColores.length || !this.subProducto.idTallas.length)) return;

  this.pasoActual++;

  if (this.pasoActual === 2) {
    this.mostrarFormProducto = false; // 👈 colapsa el form de producto al entrar a variantes
  }
}
toggleFormProducto() {
  this.mostrarFormProducto = !this.mostrarFormProducto;
}

pasoAnterior() {
  if (this.pasoActual > 1) this.pasoActual--;
}

irAPaso(n: number) {
  if (n < this.pasoActual) this.pasoActual = n; // solo permite retroceder haciendo clic en el stepper
}

toggleTallaCard(idTalla: number) {
  const existente = this.subProducto.idTallas.find((t: any) => t.idTalla === idTalla);
  if (existente) {
    this.subProducto.idTallas = this.subProducto.idTallas.filter((t: any) => t.idTalla !== idTalla);
  } else {
    const nueva = { idTalla, largoPantalon: null, entrepierna: null };
    this.subProducto.idTallas.push(nueva);
    this.abrirDrawerTalla(nueva);
  }
}

abrirDrawerTalla(talla: any) {
  this.tallaEnEdicion = talla;
  this.mostrarDrawerTalla = true;
}

cerrarDrawerTalla() {
  this.mostrarDrawerTalla = false;
  this.tallaEnEdicion = null;
}

tallaConfigurada(idTalla: number): boolean {
  const t = this.subProducto.idTallas.find((x: any) => x.idTalla === idTalla);
  return !!(t && t.largoPantalon && t.entrepierna);
}

colorNombre(id: number): string {
  return this.color.find((c: any) => c.idColor === id)?.nombreColor ?? '';
}

get totalVariantes(): number {
  return this.subProducto.idColores.length * this.subProducto.idTallas.length;
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
abrirModalTalla() {
  this.mostrarModalTalla = true;
}

cerrarModalTalla() {
  this.mostrarModalTalla = false;
  this.nuevaTalla = {
    nombreTalla: '',
    abreviatura: ''
  };
}

actualizarAbreviaturaTalla() {
  if (!this.nuevaTalla.nombreTalla) return;
  this.nuevaTalla.abreviatura = this.generarAbreviaturaTalla(this.nuevaTalla.nombreTalla);
}

generarAbreviaturaTalla(valor: string): string {
  const limpio = valor.trim().toUpperCase();

  if (/^\d+$/.test(limpio) && limpio.length <= 2) {
    return limpio;
  }

  const palabras = limpio.split(' ').filter(p => p.length > 0);

  if (palabras.length > 1) {
    const p1 = palabras[0].substring(0, 3);
    const p2 = palabras[1].substring(0, 3);
    return `${p1}-${p2}`;
  }

  return limpio.substring(0, 3);
}

guardarNuevaTalla() {
  if (!this.nuevaTalla.nombreTalla || !this.nuevaTalla.abreviatura) {
    Swal.fire('Atención', 'Completa el nombre de la talla', 'warning');
    return;
  }

  const nombreTallaCreada = this.nuevaTalla.nombreTalla;

  this.tallaService.AddTalla(this.nuevaTalla).subscribe({
    next: () => {
      this.productoService.getTalla().subscribe(tallas => {
        this.talla = tallas;

        const tallaNueva = tallas.find(
          (x: Talla) => x.nombreTalla === nombreTallaCreada
        );

        if (tallaNueva) {
          this.toggleTallaCard(tallaNueva.idTalla);
        }

        this.cerrarModalTalla();

        Swal.fire({
          icon: 'success',
          title: 'Talla registrada'
        });
      });
    },
    error: () => {
      Swal.fire('Error', 'No se pudo registrar la talla', 'error');
    }
  });
}
get coloresFiltrados(): Color[] {

  if (!this.textoBuscarColor.trim()) {
    return this.color;
  }

  return this.color.filter(c =>
    c.nombreColor
      .toLowerCase()
      .includes(this.textoBuscarColor.toLowerCase())
  );
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
  this.productoSeleccionado = null;

  // 1. CERRAR EL DRAWER DE EDICIÓN
  this.mostrarDrawerEditar = false;

  // 2. LIMPIAR EL OBJETO EN EDICIÓN (Opcional pero muy recomendado)
  this.editarSubProducto = {
    idSubProducto: 0,
    idProductoPrincipal: 0,
    idColor: null,
    idTalla: null,
    precioCompra: 0,
    precioVenta: 0,
    precioVentaPorMayor: 0,
    precioVentaLiquidacion: 0,
    largoPantalon:0,
    entrepierna:0

  };
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
abrirDrawerEditar(sub: any) {

  this.editarSubProducto = {
    idSubProducto: sub.idSubProducto,
    idProductoPrincipal: this.productoSeleccionado.idProductoPrincipal,
    idColor: sub.idColor,
    idTalla: sub.idTalla,
    precioCompra: sub.precioCompra,
    precioVenta: sub.precioVenta,
    precioVentaPorMayor: sub.precioVentaPorMayor,
    precioVentaLiquidacion: sub.precioVentaLiquidacion,
    largoPantalon:sub.largoPantalon,
    entrepierna:sub.entrepierna
  };

  this.mostrarDrawerEditar = true;

}
cerrarDrawer(){

   this.mostrarDrawerEditar=false;

}
// toggleTallaSeleccionada(idTalla: number) {
//   const index = this.subProducto.idTallas.indexOf(idTalla);
//   if (index > -1) {
//     this.subProducto.idTallas.splice(index, 1);
//   } else {
//     this.subProducto.idTallas.push(idTalla);
//   }
// }
toggleTallaSeleccionada(idTalla:number){
const index=this.subProducto.idTallas.findIndex(
x=>x.idTalla===idTalla

);

if(index>-1){

this.subProducto.idTallas.splice(index,1);

}else{

this.subProducto.idTallas.push({

idTalla:idTalla,

largoPantalon:0,

entrepierna:0

});

}

}
obtenerTextoTallasSeleccionadas(): string {
  if (!this.subProducto.idTallas || this.subProducto.idTallas.length === 0) {
    return 'Selecciona tallas';
  }
  if (this.subProducto.idTallas.length === 1) {
    const talla = this.talla.find(t=>t.idTalla===this.subProducto.idTallas[0].idTalla);
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
estaTallaSeleccionada(idTalla: number): boolean {
  return this.subProducto.idTallas.some(
    (x: any) => x.idTalla === idTalla
  );
}
obtenerNombreTalla(idTalla:number):string{

const talla=this.talla.find(x=>x.idTalla===idTalla);

return talla? talla.nombreTalla : '';

}

verSubProducto(prod: ProductoPrincipal) {
  console.log(prod.idProductoPrincipal)
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
get productoSeleccionadoVariante(): ProductoPrincipal | null {
  if (!this.subProducto.idProductoPrincipal) return null;
  return this.productoPrincipal.find(
    p => p.idProductoPrincipal === this.subProducto.idProductoPrincipal
  ) ?? null;
}
resetFormularioVariante() {
  this.pasoActual = 1;
 this.mostrarFormProducto = true;
  this.subProducto = {
    idSubProducto: null,
    idColores: [] as number[],
    idProductoPrincipal: null,
    idTallas: [] as TallaSeleccionada[],
    precioCompra: '',
    precioVenta: '',
    precioVentaPorMayor: '',
    precioVentaLiquidacion: ''
  };

  this.textoBuscarColor = '';
  this.textoBuscarTalla = '';
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
  this.subProducto.idTallas.forEach((tallaSel:TallaSeleccionada)=>{
    registros.push({
      idProductoPrincipal: this.subProducto.idProductoPrincipal,
      idColor,
      idTalla:tallaSel.idTalla,
      precioCompra: this.subProducto.precioCompra,
      precioVenta: this.subProducto.precioVenta,
      precioVentaPorMayor: this.subProducto.precioVentaPorMayor,
      precioVentaLiquidacion: this.subProducto.precioVentaLiquidacion,
     largoPantalon:tallaSel.largoPantalon,
entrepierna:tallaSel.entrepierna

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
          this.resetFormularioVariante(); // 👈 reemplaza los 2 resets sueltos
        }
      },
      error: (err) => {
        fallidos++;
        console.error('❌ Error registrando talla:', err);
      },
    });
  });

}
// abrirEditarSubProducto(sub: any) {

//   this.editarSubProducto = {
//       idSubProducto: sub.idSubProducto,
//       idProductoPrincipal: this.productoSeleccionado.idProductoPrincipal,
//       idColor: sub.idColor,
//       idTalla: sub.idTalla,
//       precioCompra: sub.precioCompra,
//       precioVenta: sub.precioVenta,
//       precioVentaPorMayor: sub.precioVentaPorMayor,
//       precioVentaLiquidacion: sub.precioVentaLiquidacion,
//       largoPantalon: sub.largoPantalon,
//     entrepierna: sub.entrepierna
//   };
//   this.mostrarModalEditarSubProducto = true;

// }
guardarEdicionSubProducto(){
  this.productoService.actualizarSubProducto(this.editarSubProducto)
  .subscribe({
    next:()=>{
      Swal.fire({icon:'success',title:'Actualizado'});
      this.mostrarDrawerEditar = false;   // 👈 corregido
      this.verSubProducto(this.productoSeleccionado);
    },
    error:(err)=>{
      Swal.fire({icon:'error',title:'Error',text:err.error.message});
    }
  });
}
// eliminarSubProducto(sub:any){
//   console.log(this.productoSeleccionado)
// Swal.fire({title:'¿Eliminar variante?',text:'Esta acción no se puede deshacer.',icon:'warning',
// showCancelButton:true,confirmButtonText:'Sí, eliminar',cancelButtonText:'Cancelar'
// }).then(result=>{
// if(result.isConfirmed){
// this.productoService.eliminarSubProducto(sub.idSubProducto).subscribe({
// next:()=>{
// Swal.fire({icon:'success',title:'Eliminado'});
// this.verSubProducto(this.productoSeleccionado);
// }});
// }
// });
// }
eliminarSubProducto(sub: any) {
  const productoActual = this.productoSeleccionado;

  Swal.fire({
    title: '¿Eliminar variante?',
    text: 'Esta acción no se puede deshacer.',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Sí, eliminar',
    cancelButtonText: 'Cancelar',
    confirmButtonColor: '#ef4444',
    cancelButtonColor: '#6b7280'
  }).then(result => {
    if (result.isConfirmed) {
      this.productoService.eliminarSubProducto(sub.idSubProducto).subscribe({
        next: () => {
          Swal.fire({ icon: 'success', title: 'Eliminado' });

          // Actualización optimista: quita la fila del array local YA,
          // sin depender de que el GET posterior tenga éxito
          this.subProductos = this.subProductos.filter(
            (s: any) => s.idSubProducto !== sub.idSubProducto
          );

          // Intentamos refrescar desde el backend por si hay más cambios,
          // pero ya no dependemos de esto para que la UI se vea correcta
          this.productoService.getSubProductosPorProducto(productoActual.idProductoPrincipal).subscribe({
            next: (resp) => {
              this.subProductos = resp ?? [];
            },
            error: (err) => {
              console.error('Error al refrescar subproductos (se ignora, ya se actualizó localmente):', err);
            }
          });
        },
        error: (err) => {
          console.error("Error al eliminar subproducto:", err);
          Swal.fire('Error', 'No se pudo eliminar la variante', 'error');
        }
      });
    }
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
guardarNuevoColor() {

  const nombreColorCreado = this.nuevoColor.nombreColor;

  this.colorService.registrarColor(this.nuevoColor).subscribe({

    next: () => {

      this.productoService.getColor().subscribe(colores => {

        this.color = colores;

        const colorNuevo = colores.find(
          x => x.nombreColor === nombreColorCreado
        );

        if (colorNuevo) {
          this.subProducto.idColores.push(colorNuevo.idColor);
        }

        this.cerrarModalColor();

        Swal.fire({
          icon: 'success',
          title: 'Color registrado'
        });

      });

    }
  });

}

}
