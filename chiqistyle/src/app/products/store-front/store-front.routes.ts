import { Routes } from "@angular/router";
import { NotFoundPageComponent } from "./pages/not-found-page/not-found-page.component";
import { LoginPageComponent } from "@auth/pages/login-page/login-page.component";
import { CatalogoPageComponent } from "./pages/catalogo-page/catalogo-page.component";
import { DetalleProductoPageComponent } from "./pages/detalle-producto-page/detalle-producto-page.component";

export const storeFrontRoutes: Routes = [
  {
    path: '',
    component: CatalogoPageComponent
  },
  {
    path: 'producto/:id',
    component: DetalleProductoPageComponent
  },
  {
    path: 'login',
    component: LoginPageComponent
  },
  {
    path: '**',
    component: NotFoundPageComponent
  }
];

export default storeFrontRoutes;
