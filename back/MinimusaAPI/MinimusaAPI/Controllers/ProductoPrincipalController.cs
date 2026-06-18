using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/ProductoPrincipal")]
    public class ProductoPrincipalController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<MProductoPrincipal>>> ListaProductos()
        {
            var funcion = new DProductoPrincipal();
            var lista = await funcion.MostrarProductos();
            return lista;
        }

        [HttpPost]
        public async Task<IActionResult> InsertarProducto([FromBody] MProductoPrincipal parametros)
        {
            var funcion = new DProductoPrincipal();
            await funcion.InsertarProducto(parametros);

            return Ok(new
            {
                message = "Producto insertado correctamente",
                producto = parametros
            });
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No se subió ninguna imagen");

            // Carpeta donde se guardan las imágenes
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Nombre único
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Devolvemos la ruta relativa para BD
            var relativePath = "/uploads/" + fileName;

            return Ok(new { path = relativePath });
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> EditarProducto(int id, [FromBody] MProductoPrincipal parametros)
        {
            var funcion = new DProductoPrincipal();
            parametros.IdProductoPrincipal = id;
            await funcion.EditarProducto(parametros);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarProducto(int id)
        {
            var funcion = new DProductoPrincipal();
            var parametros = new MProductoPrincipal { IdProductoPrincipal = id };
            await funcion.EliminarProducto(parametros);
            return NoContent();
        }
    }
}
