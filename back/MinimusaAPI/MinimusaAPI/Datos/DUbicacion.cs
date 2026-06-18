using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;

namespace MinimusaAPI.Datos
{
    public class DUbicacion
    {
        ConexionBD cn = new ConexionBD();

        public async Task<List<MUbicacion>> ListarUbicaciones()
        {
            var lista = new List<MUbicacion>();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_listarUbicacion", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await sql.OpenAsync();

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            var m = new MUbicacion
                            {
                                idUbicacion = (int)dr["IdUbicacion"],
                                nombreUbicacion = dr["NombreUbicacion"].ToString()
                            };
                            lista.Add(m);
                        }
                    }
                }
            }
            return lista;
        }
    }
}
