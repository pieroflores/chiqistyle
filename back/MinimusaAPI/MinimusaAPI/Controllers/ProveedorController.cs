using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/proveedor")]
    public class ProveedorController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<MProveedor>>> ListaProveedorGET()
        {
            var funcion = new Dproveedor();
            var lista = await funcion.MostrarProveedor();
            return lista;
        }
        [HttpPost]
        public async Task InsertarProveedorPost([FromBody] MProveedor parametros)
        {
            var funcion = new Dproveedor();
            await funcion.InsertarProveedor(parametros);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProveedor(int id, [FromBody] MProveedor parametros)
        {
            try
            {
                var funcion = new Dproveedor();
                parametros.IdProveedor = id;

                await funcion.EditarProveedor(parametros);

                return NoContent(); // 204 OK
            }
            catch (Exception ex)
            {
                // DEVUELVE el error real al frontend
                return BadRequest(new
                {
                    mensaje = "Error al editar proveedor",
                    detalle = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarProveedor(int id)
        {
            Console.WriteLine($"Eliminando Proveedor con ID: {id}"); // 👈 para verificar
            var funcion = new Dproveedor();
            var parametros = new MProveedor();
            parametros.IdProveedor = id;
            await funcion.EliminarProveedor(parametros);
            return NoContent();

        }

    }
}
