import { Component, inject } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

interface ModuloMenu {
  nombre: string;
  ruta: string;
  icono: string;
  categoria: string;
}

@Component({
  selector: 'app-admin-dashboard-layout',
  imports: [CommonModule, RouterModule], // 👈 AÑADE ESTO
  templateUrl: './admin-dashboard-layout.component.html',
})
export class AdminDashboardLayoutComponent {
  auth = inject(AuthService);

  modulosDisponibles: ModuloMenu[] = [];

  // Categorías principales del menú
  categorias: string[] = ['Catálogo', 'Operaciones', 'Reportes', 'Configuración'];

  ngOnInit() {
    this.cargarModulos();
  }

  cargarModulos() {
    const usuario = this.auth.getUsuario();
    if (usuario && usuario.modulo) {
      // aquí puedes mapear los módulos según tu backend
      this.modulosDisponibles = usuario.modulo.map((m: any) => ({
        nombre: m.nombreModulo,
        ruta: this.obtenerRutaModulo(m.nombreModulo),
        icono: this.obtenerIconoModulo(m.nombreModulo),
        categoria: this.obtenerCategoriaModulo(m.nombreModulo),
      }));
    }
  }

  obtenerRutaModulo(nombre: string): string {
    const rutas: Record<string, string> = {
      Productos: '/admin/Productos',
      Compras: '/admin/compra',
      Ventas: '/admin/venta',
      Pagos: '/admin/gestion-pagos-pendientes',
      Almacen: '/admin/gestion-inventario-stock',
      Reportes: '/admin/dashboard',
      Usuarios: '/admin/usuarios',
      Cliente: '/admin/cliente',
      Proveedor: '/admin/proveedor',
      Color: '/admin/color',
      ReporteVenta: '/admin/reporte-ventas-page',
      ReporteCompra:'/admin/reporte-compras-page',
      ReportePagosClientes:'/admin/reporte-pago-cliente-page',

    };
    return rutas[nombre] || '/admin/dashboard';
  }

  obtenerIconoModulo(nombre: string): string {
    const iconos: Record<string, string> = {
      Productos: 'fa-shirt',
      Compras: 'fa-truck',
      Ventas: 'fa-cart-shopping',
      Pagos: 'fa-coins',
      Almacen: 'fa-boxes-stacked',
      Reportes: 'fa-chart-line',
      Usuarios: 'fa-user',
    };
    return iconos[nombre] || 'fa-folder';
  }

  usuario = this.auth.getUsuario();

  tieneCategoria(categoria: string): boolean {
    return this.modulosDisponibles.some(m => m.categoria === categoria);
  }

  obtenerModulosPorCategoria(categoria: string): ModuloMenu[] {
    return this.modulosDisponibles.filter(m => m.categoria === categoria);
  }
  tieneModulo(nombre: string): boolean {
  const modulos = this.usuario?.modulo?.map((m: any) => m.nombreModulo.toLowerCase()) || [];
  return modulos.some((m: string) => m.includes(nombre.toLowerCase()));
}

  // ✅ Saber si tiene algún módulo dentro de un grupo (para abrir/cerrar el <details>)
  tieneModuloGrupo(modulosGrupo: string[]): boolean {
    return modulosGrupo.some((nombre) => this.tieneModulo(nombre));
  }
  obtenerCategoriaModulo(nombre: string): string {
  const n = nombre.toLowerCase();

  // Catálogo
  if (['productos', 'cliente', 'proveedor', 'almacen', 'color', 'talla', 'categoria'].some(x => n.includes(x))) {
    return 'Catálogo';
  }

  // Operaciones
  if (['compras', 'ventas', 'pagos', 'pagosp', 'gestiónstockinve'].some(x => n.includes(x))) {
    return 'Operaciones';
  }

  // Reportes
  if (['reporte', 'estadistica', 'grafico','ReporteVenta','ReporteCompra','ReportePagosClientes'].some(x => n.includes(x))) {
    return 'Reportes';


  }

  // Configuración o Usuarios
  if (['usuario', 'configuracion', 'rol', 'permiso'].some(x => n.includes(x))) {
    return 'Configuración';
  }

  return 'Otros';
}

  cerrarSesion() {
    this.auth.logout();
    window.location.href = '/login';
  }
}
