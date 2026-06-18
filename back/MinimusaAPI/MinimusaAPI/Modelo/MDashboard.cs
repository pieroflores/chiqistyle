namespace MinimusaAPI.Modelo
{
    public class MResumenDashboard
    {
        public int TotalProductos { get; set; }
        public int TotalVentas { get; set; }
        public int TotalCompras { get; set; }
        public decimal TotalPagos { get; set; }

        public List<VentaMes> VentasPorMes { get; set; } = new();
        public List<InventarioCategoria> InventarioPorCategoria { get; set; } = new();
        public List<UltimaVenta> UltimasVentas { get; set; } = new();
    }

    public class VentaMes
    {
        public string Mes { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class InventarioCategoria
    {
        public string Categoria { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }

    public class UltimaVenta
    {
        public int IdVenta { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
    }
}
