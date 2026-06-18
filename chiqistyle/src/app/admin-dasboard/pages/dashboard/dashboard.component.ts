import { Component, OnInit, AfterViewInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '@products/services/dashboard.service';
import { Chart, registerables } from 'chart.js';

Chart.register(...registerables);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
})
export class DashboardComponent implements OnInit, AfterViewInit {
  dashboardService = inject(DashboardService);
  cargando = true;

  resumen = {
    totalProductos: 0,
    totalVentas: 0,
    totalCompras: 0,
    totalPagos: 0,
    ventasPorMes: [] as { mes: string; total: number }[],
    inventarioPorCategoria: [] as { categoria: string; cantidad: number }[],
    ultimasVentas: [] as any[],
  };

  chartVentas: any;
  chartInventario: any;

  ngOnInit(): void {
    this.cargarResumen();
  }

  ngAfterViewInit(): void {
    // Nada aquí, solo garantizamos que el DOM está listo
  }

  cargarResumen() {
    this.dashboardService.obtenerResumen().subscribe({
      next: (data) => {
        this.resumen = data;
        this.cargando = false;

        // Esperamos un breve tiempo a que Angular pinte los <canvas>
        setTimeout(() => {
          this.generarGraficoVentas();
          this.generarGraficoInventario();
        }, 300);
      },
      error: (err) => {
        console.error('Error al cargar el dashboard:', err);
        this.cargando = false;
      },
    });
  }

  generarGraficoVentas() {
    const ctx = document.getElementById('chartVentasMes') as HTMLCanvasElement;
    if (!ctx) return; // seguridad extra

    if (this.chartVentas) this.chartVentas.destroy();

    this.chartVentas = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: this.resumen.ventasPorMes.map((v) => v.mes),
        datasets: [
          {
            label: 'Ventas por Mes (S/)',
            data: this.resumen.ventasPorMes.map((v) => v.total),
            backgroundColor: '#3B82F6',
            borderRadius: 6,
          },
        ],
      },
      options: {
        responsive: true,
        plugins: { legend: { display: false } },
        scales: {
          y: { beginAtZero: true, ticks: { color: '#374151' } },
          x: { ticks: { color: '#374151' } },
        },
         maintainAspectRatio: false, // 👈 evita el efecto de agrandarse
    resizeDelay: 200, // 👈 espera antes de recalcular
      },

    });
  }

  generarGraficoInventario() {
    const ctx = document.getElementById('chartInventario') as HTMLCanvasElement;
    if (!ctx) return;

    if (this.chartInventario) this.chartInventario.destroy();

    this.chartInventario = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: this.resumen.inventarioPorCategoria.map((c) => c.categoria),
        datasets: [
          {
            data: this.resumen.inventarioPorCategoria.map((c) => c.cantidad),
            backgroundColor: ['#22C55E', '#F59E0B', '#3B82F6', '#EF4444', '#8B5CF6'],
            borderWidth: 1,
          },
        ],
      },
      options: {
        responsive: true,
        plugins: { legend: { position: 'bottom' } },
         maintainAspectRatio: false, // 👈 evita el efecto de agrandarse
    resizeDelay: 200, // 👈 espera antes de recalcular
      },

    });
  }
}
