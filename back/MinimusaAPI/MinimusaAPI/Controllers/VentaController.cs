using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Minimusa.Datos;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MinimusaAPI.Servicios;
using MinimusaAPI.ViewModels;
using System.Runtime.ExceptionServices;
using MinimusaAPI.Conexion;
using System.Data.SqlClient;
using System.Data;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/Venta")]  // 👈 Esto es clave: define la ruta base como api/Venta
    public class VentaController : ControllerBase
    {
        [HttpGet("ConsultarProducto/{idSubProducto}")]
        public async Task<ActionResult<List<MConsultaProductoVenta>>> ConsultarProducto(int idSubProducto)
        {
            var funcion = new DVenta();
            var lista = await funcion.ConsultarProductoVenta(idSubProducto);

            if (lista == null || lista.Count == 0)
                return NotFound("No se encontraron datos para el producto seleccionado.");

            return Ok(lista);
        }
        [HttpGet("ListarPendientes")]
        public async Task<ActionResult<List<MVentaPendiente>>> ListarPendientes()
        {
            var funcion = new DVenta();
            var lista = await funcion.ListarVentasPendientes();

            if (lista == null || lista.Count == 0)
                return NotFound("No hay ventas pendientes o reservadas.");

            return Ok(lista);
        }
        [HttpGet("ObtenerDetalleVenta/{idVenta}")]
        public async Task<ActionResult<List<MDetalleVentaId>>> ObtenerDetalleVenta(int idVenta)
        {
            var funcion = new DVenta();
            var lista = await funcion.ObtenerDetalleVenta(idVenta);

            if (lista == null || lista.Count == 0)
                return NotFound("No se encontraron productos para esta venta.");

            return Ok(lista);
        }
        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] MVenta parametros)
        {
            if (parametros == null || parametros.Detalle == null || parametros.Detalle.Count == 0)
                return BadRequest("La venta debe contener al menos un producto.");

            var funcion = new DVenta();
            int idVenta = await funcion.RegistrarVenta(parametros);

            //comentar  
           /* Cabecera cabecera = await funcion.ObtenerCabeceraPorVenta(idVenta);

            if (cabecera == null)
                return BadRequest("No se pudo generar la cabecera.");

            // ✅ ESTA ES LA FORMA CORRECTA
            var sunat = new SUNAT_UTIL();
            sunat.GenerarComprobanteFB_XML(cabecera);

            return Ok(new
            {
                mensaje = "Venta registrada y enviada a SUNAT",
                idVenta
            });
             */
            //hasta aqui
             return Ok(new { mensaje = "Venta registrada correctamente", idVenta });
        }

        [HttpPost("uploadComprobante")]
        public async Task<IActionResult> UploadComprobante(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se subió ningún comprobante.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "comprobantes");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativePath = "/comprobantes/" + fileName;
            return Ok(new { path = relativePath });
        }
        [HttpPost("RegistrarPagoAdicional")]
        public async Task<IActionResult> RegistrarPagoAdicional([FromBody] MPagoAdicional parametros)
        {
            if (parametros == null || parametros.Monto <= 0)
                return BadRequest("Los datos del pago son inválidos.");

            var funcion = new DVenta();
            await funcion.RegistrarPagoAdicional(parametros);

            return Ok(new { mensaje = "Pago adicional registrado correctamente" });
        }

        [HttpGet("pagos/{idVenta}")]
        public IActionResult ListarPagosPorVenta(int idVenta)
        {
            try
            {
                var funcion = new VentaDatos();
                var lista = funcion.ListarPagosPorVenta(idVenta);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener pagos", error = ex.Message });
            }
        }
        [HttpGet("clientes-deuda")]
        public async Task<ActionResult<List<MClienteDeuda>>> ObtenerClientesDeuda()
        {
            var funcion = new DVenta();
            var data = await funcion.ListarClientesConDeuda();
            return Ok(data);
        }
        [HttpPost("pagar-cliente")]
        public async Task<IActionResult> PagarCliente([FromBody] PagoClienteDTO dto)
        {
            if (dto == null || dto.Monto <= 0)
                return BadRequest("Monto inválido");

            using (var sql = new SqlConnection(new ConexionBD().cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_PagarVentasCliente", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdCliente", dto.IdCliente);
                    cmd.Parameters.AddWithValue("@Monto", dto.Monto);
                    cmd.Parameters.AddWithValue("@Metodo", dto.Metodo ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comprobante", dto.Comprobante ?? (object)DBNull.Value);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return Ok(new { mensaje = "Pago registrado correctamente" });
        }
    }
}
