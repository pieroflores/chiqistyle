using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        [HttpGet("resumen")]
        public async Task<IActionResult> ObtenerResumen()
        {
            try
            {
                var funcion = new DDashboard();
                var data = await funcion.ObtenerResumenAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el dashboard", detalle = ex.Message });
            }
        }
    }
}
