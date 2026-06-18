using Microsoft.AspNetCore.Mvc;
using MinimusaAPI.Datos;
using MinimusaAPI.Modelo;
using static MinimusaAPI.Modelo.MCCompra;

namespace MinimusaAPI.Controllers
{
    [ApiController]
    [Route("api/Compra")]
    public class CompraController : ControllerBase
    {
        [HttpGet("Combos")]
        public async Task<ActionResult<MDatosComboCompra>> CargarCombosGET()
        {
            var funcion = new DCompra();
            var data = await funcion.CargarCombos();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> InsertarCompraPost([FromBody] MCompra parametros)
        {
            var funcion = new DCompra();

            foreach (var det in parametros.detalle)
            {
                await funcion.InsertarCompra(new MCompra
                {
                    idProveedor = parametros.idProveedor,
                    fechaCompra = parametros.fechaCompra,
                    idUsuario = parametros.idUsuario,
                    documentoReferencia = parametros.documentoReferencia,
                    observacion = parametros.observacion,
                    detalle = new List<MCompraDetalle> {
                    new MCompraDetalle {
                    idSubProducto = det.idSubProducto,
                    cantidad = det.cantidad,
                    precioUnitario = det.precioUnitario,
                    idAlmacen = det.idAlmacen
                }
            }
                });
            }

            return Ok(new { message = "Compra registrada correctamente" });
        }
        [HttpGet("ObtenerSubProductos/{idProducto}")]
        public IActionResult ObtenerSubProductos(int idProducto)
        {
            var funcion = new DCompra();

            var lista = funcion.ObtenerSubProductos(idProducto);
            return Ok(lista);
        }

    }
}
