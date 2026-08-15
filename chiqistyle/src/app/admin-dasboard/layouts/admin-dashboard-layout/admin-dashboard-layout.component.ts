import { Component, inject, OnDestroy, OnInit } from '@angular/core';
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
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-dashboard-layout.component.html',
})
export class AdminDashboardLayoutComponent implements OnInit, OnDestroy {
  auth = inject(AuthService);

  modulosDisponibles: ModuloMenu[] = [];
  categorias: string[] = ['Catálogo', 'Operaciones', 'Reportes', 'Configuración'];

  // ✅ Estado del sidebar
  isSidebarCollapsed = false;
  isMobileView = false;

  private readonly sidebarStorageKey = 'admin-sidebar-collapsed';

  ngOnInit(): void {
    this.loadSidebarPreference();
    this.updateMobileView();
    this.cargarModulos();
    window.addEventListener('resize', this.onResize);
  }

  ngOnDestroy(): void {
    window.removeEventListener('resize', this.onResize);
  }

  private updateMobileView(): void {
    this.isMobileView = window.innerWidth < 768;
  }

  private onResize = (): void => {
    this.updateMobileView();
  };

  private loadSidebarPreference(): void {
    const savedValue = localStorage.getItem(this.sidebarStorageKey);

    if (savedValue !== null) {
      this.isSidebarCollapsed = savedValue === 'true';
      return;
    }

    // Comportamiento original esperado:
    // móvil inicia colapsado; desktop inicia visible
    this.isSidebarCollapsed = window.innerWidth < 768;
  }

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
    localStorage.setItem(this.sidebarStorageKey, String(this.isSidebarCollapsed));
  }

  cargarModulos() {
    const usuario = this.auth.getUsuario();
    if (usuario && usuario.modulo) {
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
      Talla: '/admin/talla',
      Categoria: '/admin/categoria',
      ReporteVenta: '/admin/reporte-ventas-page',
      ReporteCompra: '/admin/reporte-compras-page',
      ReportePagosClientes: '/admin/reporte-pago-cliente-page',
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
      Cliente: 'fa-user',
      Proveedor: 'fa-truck-field',
      Color: 'fa-palette',
      Talla: 'fa-ruler',
      Categoria: 'fa-tags',
    };

    return iconos[nombre] || 'fa-folder';
  }

  usuario = this.auth.getUsuario();

  tieneCategoria(categoria: string): boolean {
    return this.modulosDisponibles.some((m) => m.categoria === categoria);
  }

  obtenerModulosPorCategoria(categoria: string): ModuloMenu[] {
    return this.modulosDisponibles.filter((m) => m.categoria === categoria);
  }

  tieneModulo(nombre: string): boolean {
    const modulos = this.usuario?.modulo?.map((m: any) => m.nombreModulo.toLowerCase()) || [];
    return modulos.some((m: string) => m.includes(nombre.toLowerCase()));
  }

  tieneModuloGrupo(modulosGrupo: string[]): boolean {
    return modulosGrupo.some((nombre) => this.tieneModulo(nombre));
  }

  obtenerCategoriaModulo(nombre: string): string {
    const n = nombre.toLowerCase();

    if (['productos', 'cliente', 'proveedor', 'almacen', 'color', 'talla', 'categoria'].some((x) => n.includes(x))) {
      return 'Catálogo';
    }

    if (['compras', 'ventas', 'pagos', 'pagosp', 'gestiónstockinve'].some((x) => n.includes(x))) {
      return 'Operaciones';
    }

    if (['reporte', 'estadistica', 'grafico', 'reporteventa', 'reportecompra', 'reportepagoclientes'].some((x) => n.includes(x))) {
      return 'Reportes';
    }

    if (['usuario', 'configuracion', 'rol', 'permiso'].some((x) => n.includes(x))) {
      return 'Configuración';
    }

    return 'Otros';
  }

  cerrarSesion() {
    this.auth.logout();
    window.location.href = '/login';
  }
}
