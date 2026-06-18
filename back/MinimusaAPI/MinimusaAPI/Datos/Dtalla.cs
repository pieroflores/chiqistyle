using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;

namespace MinimusaAPI.Datos
{
    public class Dtalla
    {
        ConexionBD cn = new ConexionBD();
        public async Task<List<MTalla>> MostrarTalla()
        {
            var lista = new List<MTalla>();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarTallas", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var item = await cmd.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var mtalla = new MTalla();
                            mtalla.IdTalla = (int)item["IdTalla"];
                            mtalla.NombreTalla = (string)item["NombreTalla"];
                            mtalla.Abreviatura = (string)item["Abreviatura"];
                            
                            lista.Add(mtalla);
                        }
                    }
                    return lista;
                }
            }

        }
        public async Task InsertarTalla(MTalla parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_RegistrarTalla", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@NombreTalla", parametros.NombreTalla);
                    cmd.Parameters.AddWithValue("@Abreviatura", parametros.Abreviatura);
                    
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task EditarTalla(MTalla parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EditarTalla", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdTalla", parametros.IdTalla);
                    cmd.Parameters.AddWithValue("@NombreTalla", parametros.NombreTalla);
                    cmd.Parameters.AddWithValue("@Abreviatura", parametros.Abreviatura); 
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task EliminarTalla(MTalla parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EliminarTalla", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdTalla", parametros.IdTalla);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
