using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/color")]
    public class ColorController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<MColor>>> ListaColorGET()
        {
            var funcion = new Dcolor();
            var lista = await funcion.MostrarColor();
            return lista;
        }
        [HttpPost]
        public async Task InsertarColorPost([FromBody] MColor parametros)
        {
            var funcion = new Dcolor();
            await funcion.InsertarColor(parametros);

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> EditarColor(int id, [FromBody] MColor parametros)
        {
            var funcion = new Dcolor();
            parametros.idColor = id;
            await funcion.EditarColor(parametros);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarColor(int id)
        {
            Console.WriteLine($"Eliminando Color con ID: {id}"); // 👈 para verificar
            var funcion = new Dcolor();
            var parametros = new MColor();
            parametros.idColor = id;
            await funcion.EliminarColor(parametros);
            return NoContent();

        }

    }
}
