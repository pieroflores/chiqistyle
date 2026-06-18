using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{ 
    [ApiController]
    [Route("api/Reporte")]
    public class ReporteController : ControllerBase
    {
        [HttpGet("Ventas")]
        public IActionResult ReporteVentas(DateTime? fechaInicio, DateTime? fechaFin, string? estado = null, string? metodoPago = null)
        {
            try
            {
                var funcion = new DReporteDatos();
                var lista = funcion.ReporteVentas(fechaInicio, fechaFin, estado, metodoPago);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al generar el reporte", error = ex.Message });
            }
        }
        [HttpGet("Compras")]
        public async Task<IActionResult> ReporteCompras(DateTime? fechaInicio, DateTime? fechaFin, int? idProveedor = 0, string estado = "Todos")
        {
            var funcion = new DReporteDatos();
            var lista = await funcion.ReporteCompras(fechaInicio, fechaFin, idProveedor, estado);
            return Ok(lista);
        }

        [HttpGet("PagosPorCliente")]
        public async Task<ActionResult<List<ReportePagosCliente>>> ReportePagosCliente(
        [FromQuery] int? idCliente,
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin)
        {
            var funcion = new DReporteDatos();
            var idCli = idCliente ?? 0;

            var lista = await funcion.ListarPagosPorCliente(idCli, fechaInicio, fechaFin);

            if (lista == null || lista.Count == 0)
                return NotFound("No se encontraron ventas pagadas con los filtros seleccionados.");

            return Ok(lista);
        }
        [HttpGet("ObtenerDetalleCompletoVenta/{idVenta}")]
        public async Task<ActionResult<MDetalleCompletoVenta>> ObtenerDetalleCompletoVenta(int idVenta)
        {
            var funcion = new DReporteDatos();
            var detalle = await funcion.ObtenerDetalleCompletoVenta(idVenta);

            if (detalle.Pagos.Count == 0 && detalle.Productos.Count == 0)
                return NotFound("No se encontró detalle para esta venta.");

            return Ok(detalle);
        }
    }
}
