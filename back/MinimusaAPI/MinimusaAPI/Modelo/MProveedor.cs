namespace MinimusaAPI.Modelo
{
    public class MProveedor
    {
        public int IdProveedor { get; set; }

        public string nombreProveedor { get; set; } = null!;
        public string ruc { get; set; } = null!;

        public string? direccion { get; set; }
        public string? telefono { get; set; }
        public string? email { get; set; }
    }
}
