using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/Login")]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<MLogin>> Login([FromBody] LoginRequest request)
        {
            var funcion = new DLogin();
            var usuario = await funcion.LoginUsuarioAsync(request.Usuario, request.Clave);

            if (usuario == null)
                return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });

            return Ok(usuario);
        }
    }

    // Modelo auxiliar para recibir el JSON del login
    public class LoginRequest
    {
        public string Usuario { get; set; }
        public string Clave { get; set; }
    }
}
