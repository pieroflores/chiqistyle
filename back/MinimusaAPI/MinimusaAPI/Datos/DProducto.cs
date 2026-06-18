using MinimusaAPI.Conexion;
using MinimusaAPI.Modelo;
using System.Data;
using System.Data.SqlClient;

namespace MinimusaAPI.Datos
{
    public class DProductoPrincipal
    {
        ConexionBD cn = new ConexionBD();

        public async Task<List<MProductoPrincipal>> MostrarProductos()
        {
            var lista = new List<MProductoPrincipal>();
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_ListarProductosPrincipales", sql))
                {
                    await sql.OpenAsync();
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var item = await cmd.ExecuteReaderAsync())
                    {
                        while (await item.ReadAsync())
                        {
                            var mProducto = new MProductoPrincipal
                            {
                                IdProductoPrincipal = (int)item["IdProductoPrincipal"],
                                IdCategoria = (int)item["IdCategoria"],
                                NombreProducto = (string)item["NombreProducto"],
                                CodigoProducto = (string)item["CodigoProducto"],
                                FotoProducto = item["FotoProducto"] != DBNull.Value ? (string)item["FotoProducto"] : null
                            };
                            lista.Add(mProducto);
                        }
                    }
                }
            }
            return lista;
        }

        public async Task InsertarProducto(MProductoPrincipal parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_InsertarProductoPrincipal", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCategoria", parametros.IdCategoria);
                    cmd.Parameters.AddWithValue("@NombreProducto", parametros.NombreProducto);
                    cmd.Parameters.AddWithValue("@FotoProducto", (object?)parametros.FotoProducto ?? DBNull.Value);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }


        public async Task EditarProducto(MProductoPrincipal parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EditarProductoPrincipal", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProductoPrincipal", parametros.IdProductoPrincipal);
                    cmd.Parameters.AddWithValue("@IdCategoria", parametros.IdCategoria);
                    cmd.Parameters.AddWithValue("@NombreProducto", parametros.NombreProducto);
                    cmd.Parameters.AddWithValue("@CodigoProducto", parametros.CodigoProducto);
                    cmd.Parameters.AddWithValue("@FotoProducto", (object?)parametros.FotoProducto ?? DBNull.Value);

                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task EliminarProducto(MProductoPrincipal parametros)
        {
            using (var sql = new SqlConnection(cn.cadenaSQL()))
            {
                using (var cmd = new SqlCommand("sp_EliminarProductoPrincipal", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdProductoPrincipal", parametros.IdProductoPrincipal);
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
