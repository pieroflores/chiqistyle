// MinimusaAPI.Modelo/MAlmacen.cs
namespace MinimusaAPI.Modelo
{
    public class MAlmacen
    {
        // NOTA: Se mantiene camelCase para coincidir con Angular/JSON
        public int? idAlmacen { get; set; }
        public int? idUbicacion { get; set; }
        public string seccion { get; set; }
        public int? columna { get; set; }
        public int? nivel { get; set; }
        public string descripcion { get; set; }
        public string? nombreUbicacion { get; set; } // Solo para listado
    }
}