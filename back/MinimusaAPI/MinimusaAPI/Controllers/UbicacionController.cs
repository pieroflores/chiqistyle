using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/Ubicacion")]
    public class UbicacionController : ControllerBase
    {
        [HttpGet("Listar")]
        public async Task<ActionResult<List<MUbicacion>>> ListarUbicaciones()
        {
            var funcion = new DUbicacion();
            var lista = await funcion.ListarUbicaciones();
            return Ok(lista);
        }
    }
}
