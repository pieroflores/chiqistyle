namespace MinimusaAPI.Modelo
{
    public class Pago
    {
        public int IdPago { get; set; }
        public int IdVenta { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; }
        public string Comprobante { get; set; }
        public DateTime FechaPago { get; set; }
    }
    public class MClienteDeuda
    {
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; }
        public string DNI { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal TotalPendiente { get; set; }
    }
    public class PagoClienteDTO
    {
        public int IdCliente { get; set; }
        public decimal Monto { get; set; }
        public string Metodo { get; set; }
        public string Comprobante { get; set; }
    }

}
