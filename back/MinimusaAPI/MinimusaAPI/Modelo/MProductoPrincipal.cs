namespace MinimusaAPI.Modelo
{
    public class MProductoPrincipal
    {
        public int IdProductoPrincipal { get; set; }
        public int IdCategoria { get; set; }
        public string NombreProducto { get; set; }
        public string? CodigoProducto { get; set; }
        public string? FotoProducto { get; set; } // ruta/nombre de archivo
    }
}
