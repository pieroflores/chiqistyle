using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/Cliente")]
    public class ClienteController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<MCliente>>> ListaClienteGET()
        {
            var funcion = new Dcliente();
            var lista = await funcion.MostrarCliente();
            return lista;
        }
        [HttpPost]
        public async Task InsertarClientePost([FromBody] MCliente parametros)
        {
            var funcion = new Dcliente();
            await funcion.InsertarCliente(parametros);

        }
        [HttpPut("{id}")]
        public async Task<ActionResult> EditarCliente(int id, [FromBody] MCliente parametros)
        {
            var funcion = new Dcliente();
            parametros.IdCliente = id;
            await funcion.EditarCliente(parametros);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> EliminarCliente(int id)
        {
            Console.WriteLine($"Eliminando Cliente con ID: {id}"); // 👈 para verificar
            var funcion = new Dcliente();
            var parametros = new MCliente();
            parametros.IdCliente = id;
            await funcion.EliminarCliente(parametros);
            return NoContent();

        }
        [HttpGet("dni/{numero}")]
        public async Task<IActionResult> BuscarDni(string numero)
        {
            using var http = new HttpClient();

            // 🔹 API 1
            var url1 = $"https://api.apis.net.pe/v1/dni?numero={numero}";
            var resp1 = await http.GetAsync(url1);

            if (resp1.IsSuccessStatusCode)
            {
                var data1 = await resp1.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(data1) && data1.Contains("nombre"))
                    return Content(data1, "application/json");
            }

            // 🔹 API 2 (fallback)
            var url2 = $"https://apiperu.dev/api/dni/{numero}";
            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add("Authorization", "Bearer c36f2c1355f57d9e993377480e99fd2ff9c96824777da8c97365e68156ce37a9");

            var resp2 = await http.GetAsync(url2);

            if (!resp2.IsSuccessStatusCode)
                return NotFound("No se encontró el DNI");

            var data2 = await resp2.Content.ReadAsStringAsync();

            return Content(data2, "application/json");
        }

    }
}
