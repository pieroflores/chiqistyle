namespace MinimusaAPI.Modelo
{
    public class MConsultaProductoVenta
    {
        public int IdAlmacen { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal PrecioVentaPorMayor { get; set; }
        public decimal PrecioVentaLiquidacion { get; set; }
        public string UbicacionTexto { get; set; }
        public int CantidadDisponible { get; set; }
    }

}
