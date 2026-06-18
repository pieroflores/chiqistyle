// src/app/auth/pages/login-page/login-page.component.ts
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login-page.component.html',
})
export class LoginPageComponent implements OnInit {
  usuario = '';
  clave = '';

  constructor(private authService: AuthService, private router: Router) {}

  // ✅ Si ya está logueado, redirige al dashboard
  ngOnInit(): void {
    if (this.authService.estaAutenticado()) {
      this.router.navigate(['/admin/dashboard']);
    }
  }

  login() {
    if (!this.usuario || !this.clave) {
      Swal.fire({
        icon: 'warning',
        title: 'Campos incompletos',
        text: 'Debes ingresar usuario y contraseña.',
      });
      return;
    }

    this.authService.login(this.usuario, this.clave).subscribe({
      next: (resp) => {
        Swal.fire({
          icon: 'success',
          title: `Bienvenido ${resp.nombreCompleto}`,
          showConfirmButton: false,
          timer: 1500,
        });

        setTimeout(() => {
          this.router.navigate(['/admin/dashboard']);
        }, 1500);
      },
      error: () => {
        Swal.fire({
          icon: 'error',
          title: 'Error de inicio de sesión',
          text: 'Usuario o contraseña incorrectos.',
        });
      },
    });
  }
}
