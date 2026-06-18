using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/Categoria")]
    public class CategoriaController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<MCategoria>>> ListaCategoriaGET()
        {
            var funcion = new Dcategoria();
            var lista = await funcion.MostrarCategoria();
            return lista;
        }
        [HttpPost]
        public async Task InsertarCategoriaPost([FromBody] MCategoria parametros)
        {
            var funcion = new Dcategoria();
            await funcion.InsertarCategoria(parametros);

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> EditarCategoria(int id, [FromBody] MCategoria parametros)
        {
            var funcion = new Dcategoria();
            parametros.idCategoria = id;
            await funcion.EditarCategoria(parametros);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarCategoria(int id)
        {
            Console.WriteLine($"Eliminando Categoria con ID: {id}"); // 👈 para verificar
            var funcion = new Dcategoria();
            var parametros = new MCategoria();
            parametros.idCategoria = id;
            await funcion.EliminarCategoria(parametros);
            return NoContent();

        }

    }
}
