namespace MinimusaAPI.Modelo
{
    public class MReporteVenta
    {
        public int IdVenta { get; set; }
        public string NombreCliente { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal MontoPendiente { get; set; }
        public string Estado { get; set; }
        public string MetodoPago { get; set; }
        public decimal Cantidad { get; set; } 
        public decimal PrecioCompraUnitaria { get; set; }
        public decimal PrecioCompraTotal { get; set; }
    }
    public class ReporteCompras
    {
        public int IdCompra { get; set; }
        public DateTime FechaCompra { get; set; }
        public string NombreProveedor { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
    }

    public class ReportePagosCliente
    {
        public int IdVenta { get; set; }
        public decimal Total { get; set; }
        public string NombreCliente { get; set; }
        public string FechaVenta { get; set; }
    }
    public class MRegistroPagoLista
    {
        public int IdPago { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Metodo { get; set; }
        public string Comprobante { get; set; } // Ruta o nombre del comprobante
    }
    public class MDetalleCompletoVenta
    {
        public List<MRegistroPagoLista> Pagos { get; set; } = new List<MRegistroPagoLista>();
        public List<MDetalleVentaId> Productos { get; set; } = new List<MDetalleVentaId>();
    }
}
