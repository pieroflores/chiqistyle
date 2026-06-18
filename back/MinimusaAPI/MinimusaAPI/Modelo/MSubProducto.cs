namespace MinimusaAPI.Modelo
{
    public class MSubProducto
    {
        public int? IdSubProducto { get; set; }   // opcional para inserts
        public int IdProductoPrincipal { get; set; }
        public int IdColor { get; set; }
        public int IdTalla { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioVentaPorMayor { get; set; }
        public decimal PrecioVentaLiquidacion { get; set; }

        public string? CodigoSubProducto { get; set; } // string porque puede ser alfanumérico

    }
}
