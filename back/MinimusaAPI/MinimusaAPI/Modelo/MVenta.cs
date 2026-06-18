using MinimusaAPI.Datos;

namespace MinimusaAPI.Modelo
{
    public class MDetalleVenta
    {
        public int IdSubProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int UsaPrecioPorMayor { get; set; } // 🔹 NUEVA PROPIEDAD
        public int IdAlmacen { get; set; }
    }
    public class MDetalleVentaId
    {
        public int IdVentaDetalle { get; set; }
        public int IdSubProducto { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoSubProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string Ubicacion { get; set; }
        public string NombreColor { get; set; }
        public string NombreTalla { get; set; }
    }

    public class MVenta
    {
        public int IdCliente { get; set; }
        public DateTime FechaVenta { get; set; }
        public int IdUsuario { get; set; }
        public decimal MontoPagado { get; set; }
        public string MetodoPago { get; set; }
        public string Comprobante { get; set; }
        public string TipoTransaccion { get; set; }

        // 👇 Lista de productos (detalle)
        public List<MDetalleVenta> Detalle { get; set; }
    }

    public class MVentaPendiente
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string DNI { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal MontoPendiente { get; set; }
        public string Estado { get; set; }
        public string MetodoPago { get; set; }
        public string Comprobante { get; set; }
        public string TipoTransaccion { get; set; }
    }
    public class MPagoAdicional
    {
        public int IdVenta { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; }
        public string Comprobante { get; set; }  // Ruta del comprobante subido
    }
}
