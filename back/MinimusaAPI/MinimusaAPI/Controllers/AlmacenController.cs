using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/Almacen")]
    public class AlmacenController : ControllerBase
    {
        [HttpGet("Listar")]
        public async Task<ActionResult<List<MAlmacen>>> ListarAlmacenes()
        {
            var funcion = new DAlmacen();
            var lista = await funcion.ListarAlmacenes();
            return Ok(lista);
        }

        [HttpPost("Registrar")]
        public async Task<ActionResult> RegistrarAlmacen([FromBody] MAlmacen parametros)
        {
            // 🚨 CAMBIO CLAVE: Verifica la validez del modelo para diagnosticar el error 400
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var funcion = new DAlmacen();
            await funcion.InsertarAlmacen(parametros);
            return Ok();
        }

        [HttpPut("Editar")]
        public async Task<ActionResult> EditarAlmacen([FromBody] MAlmacen parametros)
        {
            // 🚨 CAMBIO CLAVE: Verifica la validez del modelo para diagnosticar el error 400
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var funcion = new DAlmacen();
            await funcion.EditarAlmacen(parametros);
            return Ok();
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<ActionResult> EliminarAlmacen(int id)
        {
            var funcion = new DAlmacen();
            await funcion.EliminarAlmacen(id);
            return Ok();
        }
    }
}
