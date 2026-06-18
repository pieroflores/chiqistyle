import { Routes } from "@angular/router";
import { AdminDashboardLayoutComponent } from "./layouts/admin-dashboard-layout/admin-dashboard-layout.component";
import { ProductsAdminPageComponent } from "./pages/products-admin-page/products-admin-page.component";
import { ProveedorAdminPageComponent } from "./pages/proveedor-admin-page/proveedor-admin-page.component";
import { ClienteAdminPageComponent } from "./pages/cliente-admin-page/cliente-admin-page.component";
import { ColorAdminPageComponent } from "./pages/color-admin-page/color-admin-page.component";
import { AlmacenAdminPageComponent } from "./pages/almacen-admin-page/almacen-admin-page.component";
import { CategoriaAdminPageComponent } from "./pages/categoria-admin-page/categoria-admin-page.component";
import { TallaAdminPageComponent } from "./pages/talla-admin-page/talla-admin-page.component";
import { CompraAdminPageComponent } from "./pages/compra-admin-page/compra-admin-page.component";
import { DashboardComponent } from "./pages/dashboard/dashboard.component";
import { VentaAdminPageComponent } from "./pages/venta-admin-page/venta-admin-page.component";
import { authGuard } from "../core/guards/auth.guard";
import { GestionPagosPendienteAdminPageComponent } from "./pages/gestion-pagos-pendiente-admin-page/gestion-pagos-pendiente-admin-page.component";
import { StockListPageComponent } from "./pages/stock-list-page/stock-list-page.component";
import { ReporteVentasPageComponent } from "./pages/reporte-ventas-page/reporte-ventas-page.component";
import { ReporteComprasPageComponent } from "./pages/reporte-compras-page/reporte-compras-page.component";
import { ReportePagosClientePageComponent } from "./pages/Reporte-pagos-cliente-page/Reporte-pagos-cliente-page.component";

export const adminDashboardRoutes: Routes = [{

    path:'',
    component: AdminDashboardLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'Productos',
        component: ProductsAdminPageComponent,
      },
      {
        path:'proveedor',
        component: ProveedorAdminPageComponent
      },
      {
        path:'cliente',
        component: ClienteAdminPageComponent
      },
      {
        path:'color',
        component: ColorAdminPageComponent
      },
      {
        path:'almacen',
        component: AlmacenAdminPageComponent
      },
      {
        path:'categoria',
        component: CategoriaAdminPageComponent
      },
      {
        path:'talla',
        component: TallaAdminPageComponent
      },
      {
        path: 'compra',
         component: CompraAdminPageComponent
      },
      {
        path: 'venta',
         component: VentaAdminPageComponent
      },
      {
        path: 'gestion-pagos-pendientes',
         component: GestionPagosPendienteAdminPageComponent
      },
{
 path: 'gestion-inventario-stock',
         component: StockListPageComponent
},
      {
        path: 'dashboard',
         component: DashboardComponent
      },
      {
        path: 'reporte-ventas-page',
         component: ReporteVentasPageComponent
      },
      {
        path: 'reporte-compras-page',
         component: ReporteComprasPageComponent
      },
      {
        path: 'reporte-pago-cliente-page',
         component: ReportePagosClientePageComponent
      },
      {
        path: '**',
        redirectTo: 'dashboard',
      },
    ]
}]
export default adminDashboardRoutes;
