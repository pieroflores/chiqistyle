namespace MinimusaAPI.Modelo
{
    public class MLogin
    {
       public int idUsuario { get; set; }
        public string usuarioLogin { get; set; }
        public string nombreCompleto  { get; set; }
        public int idRol   { get; set; }
        public string nombreRol { get; set; }
        public List<MModulo> modulo { get; set; } = new List<MModulo>();
    }
    public class MModulo
    {
        public string NombreModulo { get; set; }
    }
}
