namespace MinimusaAPI.Modelo
{
    public class MStock
    {
        public int IdSubProducto { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string CodigoSubProducto { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Talla { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public string? FotoProducto { get; set; }
        public DateTime FechaCompra { get; set; }

        public decimal PrecioVentaLiquidacion { get; set; }
    }
}
