using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;
using System.Data.SqlClient;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/subproductos")]
    public class SubProductosController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<MSubProducto>>> ListaSubProductosGET()
        {
            var funcion = new DSubProducto();
            var lista = await funcion.MostrarSubProductos();
            return lista;
        }
        [HttpGet("producto/{idProductoPrincipal}")]
        public async Task<ActionResult<List<MSubProductoDTO>>> ListaSubProductosPorProducto(int idProductoPrincipal)
        {
            var funcion = new DSubProducto();
            var lista = await funcion.MostrarSubProductosPorProducto(idProductoPrincipal);

            if (lista.Count == 0)
                return NotFound(new { message = "No se encontraron subproductos para este producto." });

            return Ok(lista);
        }


        [HttpPost]
        public async Task<IActionResult> InsertarSubProductoPost([FromBody] MSubProducto parametros)
        {
            try
            {
                var funcion = new DSubProducto();
                await funcion.InsertarSubProducto(parametros);
                return Ok(new { message = "Subproducto registrado correctamente" });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) // Unique Key violation
                {
                    return Conflict(new { message = "El código del subproducto ya existe." });
                }
                throw; // otros errores se siguen propagando
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> EditarSubProducto(int id, [FromBody] MSubProducto parametros)
        {
            var funcion = new DSubProducto();
            parametros.IdSubProducto = id;
            await funcion.EditarSubProducto(parametros);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarSubProducto(int id)
        {
            var funcion = new DSubProducto();
            var parametros = new MSubProducto { IdSubProducto = id };
            await funcion.EliminarSubProducto(parametros);
            return NoContent();
        }
    }
}
