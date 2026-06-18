using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/talla")]
    public class TallaController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<MTalla>>> ListaTallaGET()
        {
            var funcion = new Dtalla();
            var lista = await funcion.MostrarTalla();
            return lista;
        }
        [HttpPost]
        public async Task InsertarTallaPost([FromBody] MTalla parametros)
        {
            var funcion = new Dtalla();
            await funcion.InsertarTalla(parametros);

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> EditarTalla(int id, [FromBody] MTalla parametros)
        {
            var funcion = new Dtalla();
            parametros.IdTalla = id;
            await funcion.EditarTalla(parametros);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarTalla(int id)
        {
            Console.WriteLine($"Eliminando talla con ID: {id}"); // 👈 para verificar
            var funcion = new Dtalla();
            var parametros = new MTalla();
            parametros.IdTalla = id;
            await funcion.EliminarTalla(parametros);
            return NoContent();

        }

    }
}
