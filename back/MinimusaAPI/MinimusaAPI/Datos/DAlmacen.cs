using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;

namespace MinimusaAPI.Datos
{
    public class DAlmacen
    {
        ConexionBD cn = new ConexionBD();

        public async Task<List<MAlmacen>> ListarAlmacenes()
        {
            var lista = new List<MAlmacen>();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                // 🔹 Mejorado: incluir nombre de ubicación si haces un JOIN
                using (var cmd = new SqlCommand(@"SELECT a.*, u.NombreUbicacion 
                                                  FROM Almacen a
                                                  INNER JOIN Ubicacion u ON a.IdUbicacion = u.IdUbicacion", sql))
                {
                    cmd.CommandType = CommandType.Text;
                    await sql.OpenAsync();

                    using (var dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            var m = new MAlmacen
                            {
                                idAlmacen = (int)dr["IdAlmacen"],
                                idUbicacion = (int)dr["IdUbicacion"],
                                seccion = dr["Seccion"].ToString(),
                                columna = Convert.ToInt32(dr["Columna"]),
                                nivel = Convert.ToInt32(dr["Nivel"]),
                                descripcion = dr["Descripcion"].ToString(),
                                nombreUbicacion = dr["NombreUbicacion"].ToString()
                            };
                            lista.Add(m);
                        }
                    }
                }
            }
            return lista;
        }

        public async Task InsertarAlmacen(MAlmacen parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_RegistrarAlmacen", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idUbicacion", parametros.idUbicacion.Value);
                    cmd.Parameters.AddWithValue("@seccion", parametros.seccion);
                    cmd.Parameters.AddWithValue("@columna", parametros.columna.Value);
                    cmd.Parameters.AddWithValue("@nivel", parametros.nivel);
                    cmd.Parameters.AddWithValue("@descripcion", parametros.descripcion);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task EditarAlmacen(MAlmacen parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EditarAlmacen", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idAlmacen", parametros.idAlmacen.Value);
                    // 🚨 CAMBIO CLAVE: Incluir idUbicacion para el SP
                    cmd.Parameters.AddWithValue("@idUbicacion", parametros.idUbicacion.Value);
                    cmd.Parameters.AddWithValue("@seccion", parametros.seccion);
                    cmd.Parameters.AddWithValue("@columna", parametros.columna.Value);
                    cmd.Parameters.AddWithValue("@nivel", parametros.nivel);
                    cmd.Parameters.AddWithValue("@descripcion", parametros.descripcion);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task EliminarAlmacen(int idAlmacen)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EliminarAlmacen", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idAlmacen", idAlmacen);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
