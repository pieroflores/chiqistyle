using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;

namespace MinimusaAPI.Datos
{
    public class Dcolor
    {
        ConexionBD cn = new ConexionBD();
        public async Task<List<MColor>> MostrarColor()
        {
            var lista = new List<MColor>();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarColor", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var item = await cmd.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var mColor = new MColor();
                            mColor.idColor = (int)item["idColor"];
                            mColor.nombreColor = (string)item["nombreColor"];
                            mColor.abreviatura = (string)item["abreviatura"];

                            lista.Add(mColor);
                        }
                    }
                    return lista;
                }
            }

        }
        public async Task InsertarColor(MColor parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_RegistrarColor", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@nombreColor", parametros.nombreColor);
                    cmd.Parameters.AddWithValue("@abreviatura", parametros.abreviatura);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task EditarColor(MColor parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EditarColor", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idColor", parametros.idColor);
                    cmd.Parameters.AddWithValue("@nombreColor", parametros.nombreColor);
                    cmd.Parameters.AddWithValue("@abreviatura", parametros.abreviatura);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task EliminarColor(MColor parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EliminarColor", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idColor", parametros.idColor);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
