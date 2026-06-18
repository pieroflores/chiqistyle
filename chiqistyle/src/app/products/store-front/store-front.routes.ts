import { Routes } from "@angular/router";
import { NotFoundPageComponent } from "./pages/not-found-page/not-found-page.component";
import { LoginPageComponent } from "@auth/pages/login-page/login-page.component";

export const storeFrontRoutes: Routes = [
  {
    path: '',
    redirectTo: 'login',   // 👈 redirección inicial al login
    pathMatch: 'full'      // 👈 asegura que solo redirija la raíz
  },
  {
    path: 'login',
    component: LoginPageComponent
  },
  {
    path: '**',
    component: NotFoundPageComponent   // 👈 muestra página 404
  }
];

export default storeFrontRoutes;
