using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/Stock")]
    public class StockController : ControllerBase
    {
        [HttpGet("listar")]
        public async Task<IActionResult> ListarStock()
        {
            var funcion = new DStock();
            var data = await funcion.ListarStockGeneralAsync();
            return Ok(data);
        }
         
    }
}
