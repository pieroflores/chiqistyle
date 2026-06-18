namespace MinimusaAPI.Modelo
{
    public class MSubProductoDTO
    {
        public int IdSubProducto { get; set; }
        public int IdProductoPrincipal { get; set; }
        public string NombreColor { get; set; } = string.Empty;
        public string NombreTalla { get; set; } = string.Empty;
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioVentaPorMayor { get; set; }
        public decimal PrecioVentaLiquidacion { get; set; }
    }
}
